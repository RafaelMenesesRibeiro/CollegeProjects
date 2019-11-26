#include <ctype.h>
#include "proj2.h"

#define MAX_HASHTAG_LEN 140

static const char separators[] = {' ','\t',',',';','.','?','!','"','\n',':','\0'};

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	executa_A
//Funcionalidade:
//A funcao executa_A processa uma mensagem introduzida. Chama a funcao fgets() e a funcao split
//(para o processamento das palavras).
//Input:	ponteiro para DataBase
//Output:	---
void executa_A(pDataBase DataBase){
	char *message = (char*) malloc((MAX_HASHTAG_LEN + 1)*sizeof(char));
	fgets(message, MAX_HASHTAG_LEN + 1, stdin);
	split(message, DataBase);
	free(message);
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	executa_S
//Funcionalidade:
//A funcao executa_S imprime o numero de hashtags distintas e o numero total de ocorrencias de
//todas as hashtags.
//Input:	ponteiro para DataBase
//Output:	---
void executa_S(pDataBase DataBase){
	if (DataBase == DATABASE_NONEXISTENT) return;
	printf("%d %d\n", get_DataBase_HashNumber(DataBase), get_DataBase_HashTotal(DataBase));
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	executa_M
//Funcionalidade:
//A funcao executa_M imprime a hashtag mais popular.
//Input:	ponteiro para DataBase
//Output:	---
void executa_M(pDataBase DataBase){
	if (DataBase == DATABASE_NONEXISTENT) return;
	pItem MostPopular = get_DataBase_MostPopular(DataBase);
	if ((MostPopular) == HASHPOINT_NONEXISTENT) return;
	else print_Item(MostPopular);
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	executa_L
//Funcionalidade:
//A funcao executa_L lista todas as hashtags por ordem decrescente do numero de ocurrencias. Em
//caso de igualdade, as hashtags sao ordenadas lexicograficamente.
//Input:	ponteiro para DataBase
//Output:	---
void executa_L(pDataBase DataBase){	
	int hash_Number = get_DataBase_HashNumber(DataBase);
	int j;
	
	//Reserva espaca para um vetor de ponteiros para Item de tamanho igual ao numero de hahstags
	//distintas existentes (como o objetivo e ordenar as hashtags, nao ha necessidade de reservar
	//mais espaco.
	pItem *array = malloc(sizeof(pItem)*hash_Number);
	
	//Copia todos os ponteiros para Item existentes na estrutura DatBase para o vetor alocado.
	traverse_HashTable(DataBase, array);
	
	//Ordena o vetor de acordo com a funcao cmp_func.
	qsort(array, hash_Number, sizeof(pItem), cmp_func);

	//Imprime as estrturas Item referenciadas pelos ponteiros do vetor ordenado.
	for (j = 0; j < hash_Number; j++) print_Item(array[j]);

	//Liberta o espaco reservado para o vetor de ordenacao
	free(array);
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	executa_X
//Funcionalidade:
//A funcao executa_X termina o programa, libertando a memoria utilizada por uma estrutura DataBase
//referenciada pelo ponteiro dado como argumento.
//Input:	ponteiro para DataBase
//Output:	---
void executa_X(pDataBase DataBase){
	if (DataBase == DATABASE_NONEXISTENT) return;
	free_HashTable(DataBase);
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	split
//Funcionalidade:
//A funcao split divide uma string dada como argumento em palavras (tokens) e processa-os
//individualmente atraves da funcao hash_process().
//Input:	string
//Output:	---
void split(char *line, pDataBase DataBase){
	char *token = strtok(line, separators);
	while (token != NULL){
		if (*token == '#') hash_process(DataBase, token);
		token = strtok(NULL, separators);
	}
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	hash_process
//Funcionalidade:
//A funcao hash_process processa um token. Verifica se a hashtag ja foi inserida. Se ja foi
//aumenta o numero de ocorrencias da hashtag em particular, e o numero de hashtags inseridas. 
//Se ainda nao foi, insere e aumenta o numero de ocorrencias, o numero de hashtags totais e o numero
//de hashtags inseridas. Em ambos os casos verifica se e necessario alterar o ponteiro para Item
//(componente <hash_MostPopular>) devido ao numero de ocorrencias da hashtag inserida.
//Input:	ponteiro para DataBase e string
//Output:	---
void hash_process(pDataBase DataBase, char *token){
	int i;
	for (i = 0; token[i] != '\0'; i++) token[i] = tolower(token[i]);
	pItem new = new_Item(token);
	pHashPoint point = search_HashTable_Item(DataBase, new), newHash = HASHPOINT_INICIALIZATION;

	if (point == HASHPOINT_NONEXISTENT){
		set_DataBase_HashTotal(DataBase, get_DataBase_HashTotal(DataBase) + 1);
		set_DataBase_HashNumber(DataBase, get_DataBase_HashNumber(DataBase) + 1);
		insert_HashTable_Item(DataBase, new);
		newHash = new_HashPoint(new);
		check_DataBase_MostPopular(DataBase, newHash);
		free(newHash);
		return;
	}
	else {
		set_DataBase_HashTotal(DataBase, get_DataBase_HashTotal(DataBase) + 1);
		pItem aux = get_HashPoint_Item(point);
		set_Item_Ocorrencias(aux, get_Item_Ocorrencias(aux) + 1);
		check_DataBase_MostPopular(DataBase, point);
		free_Item(new);		
		return;
	}
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	cmp_func
//Funcionalidade:
//A funcao cmp_func compara duas estruturas Item referenciadas pelos ponteiros dados como argumento
//considerando apenas a componente <ocorrencias> das estruturas.
//Input:	ponteiros para void
//Output:	<0 se o a primeira estrutura for menor. <0 se for maior.
int cmp_func(const void *a, const void *b){
	pItem Item1 = *((pItem*)a);
	pItem Item2 = *((pItem*)b);

	int diff = get_Item_Ocorrencias(Item2) - get_Item_Ocorrencias(Item1);
	if (diff == 0) return strcoll(get_Item_Tag(Item1), get_Item_Tag(Item2));
	return diff;
}