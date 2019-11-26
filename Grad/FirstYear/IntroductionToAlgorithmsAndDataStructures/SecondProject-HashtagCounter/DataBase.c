#include "DataBase.h"

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	hash
//Funcionalidade:
//A funcao hash recebe um ponteiro para string e calcula em que posicao da tabela de dispersao
//(componente <Heads>) e que pertence.
//Input:	ponteiro para char (texto da hashtag)
//Output:	inteiro
int hash(char *Hash_Text){
    long hash = 5381;
    int c;
    while ((c = *Hash_Text++))
        hash = ((hash << 5) + hash) + c;
    if (hash % 10007 >= 0) return hash % 10007;
    return -hash % 10007;
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	init_DataBase
//Funcionalidade:
//A funcao init_DataBase cria uma estrutura DataBase. A componente <heads> e inicializada a NULL,
//as componentes <hash_Number> e <hash_Total> a 0 e a componente
//<hash_MostPopular> a NULL.
//Input:	---
//Output:	ponteiro para DataBase
pDataBase init_DataBase(){
	int i = 0;
	pDataBase DataBase = (pDataBase) malloc(sizeof(struct DataBase));
	pHashPoint *Data_Heads = (pHashPoint*) malloc(M*sizeof(struct HashPoint));
	for (i = 0; i < M; i++) Data_Heads[i] = NULL;
	
	set_DataBase_Heads(DataBase, Data_Heads);
	set_DataBase_HashNumber(DataBase, 0);
	set_DataBase_HashTotal(DataBase, 0);
	set_DataBase_MostPopular(DataBase, HASHPOINT_INICIALIZATION);
	return DataBase;
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	get_DataBase_Heads
//Funcionalidade:
//A funcao get_DataBase_Heads retorna a componente <Heads> da estrutura DataBase referenciada pelo 
//ponteiro dado como primeiro argumento
//Input:	ponteiro para DataBase
//Output:	vetor de ponteiros para Item
pHashPoint* get_DataBase_Heads(pDataBase DataBase){
	return ((*DataBase).Heads);
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	get_DataBaseList_Head
//Funcionalidade:
//A funcao get_DataBaseList_Head retorna um ponteiro para a cabeca da lista ligada (componente <Heads>
//de uma estrutura DataBase referenciada pelo ponteiro dado como primeiro argumento) que se encontra
//na posicao cujo valor e dado como segundo argumento.
//Input:	ponteiro para DataBase e inteiro
//Output:	ponteiro para HashPoint
pHashPoint get_DataBaseList_Head(pDataBase DataBase, int i){
	return ((*DataBase).Heads)[i];
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	get_DataBase_HashNumber
//Funcionalidade:
//A funcao get_DataBase_HashNumber retorna a componente <hash_Number> de uma estrutura DataBase
//referenciada pelo ponteiro dado como argumento.
//Input:	ponteiro para DataBase
//Output:	inteiro (componente que indica o numero de hashtags distintas inseridas)
int get_DataBase_HashNumber(pDataBase DataBase){
	return (*DataBase).hash_Number;
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	get_DataBase_HashTotal
//Funcionalidade:
//A funcao get_DataBase_HashTotal retorna a componente <hash_Total> de uma estrutura DataBase
//referenciada pelo ponteiro dado como argumento.
//Input:	ponteiro para DataBase
//Output:	inteiro (componente que indica a soma de ocorrencias de todas as hashtags distintas)
int get_DataBase_HashTotal(pDataBase DataBase){
	return (*DataBase).hash_Total;
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	get_DataBase_MostPopular
//Funcionalidade:
//A funcao get_DataBase_MostPopular retorna a componente <hash_MostPopular> de uma estrutura 
//DataBase referenciada pelo ponteiro dado como argumento.
//Input:	ponteiro para DataBase
//Output:	ponteiro para Item (componente que indica o Item cuja hashtag da estrutura referenciada
//			e o mais popular)
pItem get_DataBase_MostPopular(pDataBase DataBase){
	return (*DataBase).hash_MostPopular;
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	set_DataBase_Heads
//Funcionalidade:
//A funcao set_DataBase_Heads define o valor da componente <Heads> de uma estrtura DataBase dada
//pelo primeiro argumento para o vetor de ponteiros para HashPoint dada pelo segundo argumento.
//Input:	ponteiro para DataBase e vetor de ponteiros para HashPoint
//Output:	---
void set_DataBase_Heads(pDataBase DataBase, pHashPoint *Data_Heads){
	(*DataBase).Heads = Data_Heads;
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	set_DataBaseList_Head
//Funcionalidade:
//A funcao set_DataBaseList_Head define o valor da cabeca da lista ligada encontrada na posicao dada 
//pelo inteiro dado como segundo argumento, (da componente <Heads> de uma estrtura DataBase 
//referenciada pelo ponteiro dado pelo primeiro argumento), para o valor do ponteiro para HashPoint 
//dado como terceiro argumento.
//Input:	ponteiro para DataBase, inteiro (posicao na componente <Heads>) e ponteiro para 
//			HashPoint (nova cabeca)
//Output:	---
void set_DataBaseList_Head(pDataBase DataBase, int i, pHashPoint newHead){
	((*DataBase).Heads)[i] = newHead;
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	set_DataBase_HashNumber
//Funcionalidade:
//A funcao set_DataBase_HashNumber define o valor da componente <hash_Number> de uma estrutura 
//DataBase dada pelo primeiro argumento para o valor dado como segundo argumento.
//Input:	ponteiro para DataBase e inteiro (indica o numero de hashtags distintas)
//Output:	---
void set_DataBase_HashNumber(pDataBase DataBase, int i){
	(*DataBase).hash_Number = i;
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	set_DataBase_HashTotal
//Funcionalidade:
//A funcao set_DataBase_HashTotal define o valor da componente <hash_Total> de uma estrutura 
//DataBase dada pelo primeiro argumento para o valor dado como segundo argumento.
//Input:	ponteiro para DataBase e inteiro (indica a soma de ocorrencias das hashtags distintas)
//Output:	---
void set_DataBase_HashTotal(pDataBase DataBase, int i){
	(*DataBase).hash_Total = i;
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	set_DataBase_MostPopular
//Funcionalidade:
//A funcao set_DataBase_MostPopular define o valor da componente <hash_MaisPopular> de uma estrutura 
//DataBase dada pelo primeiro argumento para o valor do ponteiro para HashPoint dado como segundo.
//Input:	ponteiro para DataBase e ponteiro para Item (indica o Item cuja hashtag da estrutura 
//			referenciada e o mais popular)
//Output:	---
void set_DataBase_MostPopular(pDataBase DataBase, pItem item){
	(*DataBase).hash_MostPopular = item;
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	search_HashTable_Item
//Funcionalidade:
//A funcao search_HashTable_Item procura uma estrutura Item referenciado pelo ponteiro dado como 
//segundo argumento numa estrutura DataBase referenciada pelo ponteiro dado como primeiro.
//Input:	ponteiro para DataBase e ponteiro para Item (que se procura)
//Output:	ponteiro para HashPoint (que referencia uma estrutura que contem um ponteiro para o
//			Item procurado)
pHashPoint search_HashTable_Item(pDataBase DataBase, pItem searchItem){
	int i = hash(get_Item_Tag(searchItem));
	return search_List_Item(get_DataBaseList_Head(DataBase, i), searchItem);
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	insert_HashTable_Item
//Funcionalidade:
//A funcao insert_HashTable_Item insere uma estrutura Item referenciado pelo ponteiro dado como 
//segundo argumento numa estrutura DataBase referenciada pelo ponteiro dado como primeiro.
//Input:	ponteiro para DataBase e ponteiro para Item (que se quer inserir)
//Output:	---
void insert_HashTable_Item(pDataBase DataBase, pItem insertItem){
	int i = hash(get_Item_Tag(insertItem));
	pHashPoint pHead = get_DataBaseList_Head(DataBase, i);
	set_DataBaseList_Head(DataBase, i, insert_List_Item(pHead, insertItem));
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	check_DataBase_MostPopular
//Funcionalidade:
//A funcao check_DataBase_MostPopular verifica se uma estrutura HashPoint referenciada pelo ponteiro
//dado como segundo argumento contem um ponteiro para Item que referencia uma estrutura que contem 
//um hashtag que deve substituir a componente <Hash_MostPopular> de uma estrtura DataBase dada como
//primeiro argumento.
//Input:	ponteiro para DataBase e ponteiro para HashPoint
//Output:	---
void check_DataBase_MostPopular(pDataBase DataBase, pHashPoint point){
	pItem mostp = get_DataBase_MostPopular(DataBase);
	pItem aux = get_HashPoint_Item(point);
	
	if (get_DataBase_MostPopular(DataBase) == ITEM_NONEXISTENT){
		set_DataBase_MostPopular(DataBase, aux);
		return;
	}

	//Verifica se as estruturas referenciadas pelos ponteiros sao iguais. Se forem, como nao e
	//necessario alterar a componente <hash_MostPopular>.
	int a = cmp_Item(mostp, aux);
	if (a == 0) return;

	//Se nao forem iguais, compara a componente <ocorrencias> das duas estruturas
	int b = get_Item_Ocorrencias(mostp) - get_Item_Ocorrencias(aux);
	
	//Se essa componente for igual, verifica se a componente <hash_MostPopular> (ponteiro para ITem)
	//e menor (em relacao a ordenacao lexicografica dos hashtags referenciados).
	if (b == 0){
		//Se for, nao e necessaria qualquer alteracao.
		if (a < 0) return;
		//Se nao for, altera a componente <hash_MostPopular> para a componente <item> da estrutura
		//referenciada pelo ponteiro dado como segundo argumento.
		else if (a > 0){
			set_DataBase_MostPopular(DataBase, aux);
			return;
		}
	}

	//Se a compoenente <ocorrencias> da componente <hash_MostPopular> for maior, nao e necessaria
	//qualquer alteracao.
	else if (b > 0) return;

	//Se for menor, altera a componente <hash_MostPopular> para a componente <item> da estrutura
	//referenciada pelo ponteiro dado como segundo argumento.
	else if (b < 0){
		set_DataBase_MostPopular(DataBase, aux);
		return;
	}
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	traverse_HashTable
//Funcionalidade:
//A funcao traverse_HashTable percorre a tabela de dispersao de uma estrutura DatBase referenciada
//pelo ponteiro dado como primeiro argumento e chama a funcao traverse_HashTableList para cada
//lista ligada.
//Input:	ponteiro para DataBase e vetor de ponteiros para Item
//Output:	---
void traverse_HashTable(pDataBase DataBase, pItem *array){
	int i, j = 0;
	for (i = 0; i < M; i++) j = traverse_HashTableList(array, get_DataBaseList_Head(DataBase, i), j);
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	traverse_HashTableList
//Funcionalidade:
//A funcao traverse_HashTableList percorre uma lista ligada e adiciona o ponteiro para Item de 
//todas as estruturas referenciadas pelos ponteiros para HashPoint da lista ligada (que nao sejam
//NULL) ao vetor de ponteiros para Item dado como segundo argumento.
//Input:	ponteiro para DataBase e vetor de ponteiros para Item
//Output:	---
int traverse_HashTableList(pItem *array, pHashPoint pHead, int j){
	pHashPoint aux;
	if (pHead == HASHPOINT_NONEXISTENT){ return j; }
	for (aux = pHead; aux != HASHPOINT_NONEXISTENT; aux = get_HashPoint_Next(aux)){
		array[j] = get_HashPoint_Item(aux);
		j++;
	}
	return j;
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	free_HashList
//Funcionalidade:
//A funcao free_HashList liberta a memoria utilizada para uma lista ligada cuja cabeca e
//referenciada pelo ponteiro para HashPoint dado como argumento.
//Input:	ponteiro para HashPoint
//Output:	---
void free_HashList(pHashPoint pHead){
	pHashPoint aux = pHead, next;
	if (pHead == HASHPOINT_NONEXISTENT) return;
	
	while(aux != HASHPOINT_NONEXISTENT){
		next = get_HashPoint_Next(aux);
		free_HashPoint(aux);
		aux = next;
	}
}


////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	free_HashTable
//Funcionalidade:
//A funcao free_HashTable liberta a memoria utilizada para uma estrutura DataBase referenciada
//pelo ponteiro dado como argumento.
//Input:	ponteiro para DataBase
//Output:	---
void free_HashTable(pDataBase DataBase){
	int i;
	for (i = 0; i < M; i++) free_HashList(get_DataBaseList_Head(DataBase, i));
	free(get_DataBase_Heads(DataBase));
	free(DataBase);
}

////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////