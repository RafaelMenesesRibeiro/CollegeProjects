#include "Item.h"

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	new_Item
//Funcionalidade:
//A funcao new_Item cria uma nova estrutura Item. O texto da nova estrutura e uma copia de texto 
//referenciado pelo ponteiro dado como argumento (atraves do strdup()) e o numero de ocorrencias e 
//inicializado a 1.
//Input:	ponteiro para char (texto da hashtag)
//Output:	ponteiro para a nova estrutura Item
pItem new_Item(char *Hash_Text){
	pItem newItem = (pItem) malloc(sizeof(struct Item));
	char *aux_Text = strdup(Hash_Text);
	set_Item_Tag(newItem, aux_Text);
	set_Item_Ocorrencias(newItem, ITEM_INICIALIZATION);
	return newItem;
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	get_Item_Tag
//Funcionalidade:
//A funcao get_Item_Tag retorna a componente <tag> de uma estrutura Item referenciada pelo
//ponteiro dado como argumento.
//Input:	ponteiro para Item
//Output:	pnnteiro para char (componente que referencia o texto da hashtag)
char* get_Item_Tag(pItem item){
	return (*item).tag;
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	get_Item_Ocorrencias
//Funcionalidade:
//A funcao get_Item_Ocorrencias retorna a componente <ocorrencias> de uma estrutura Item
//referenciada pelo ponteiro dado como argumento.
//Input:	ponteiro para Item
//Output:	inteiro (componente que indica o numero de ocorrencias da hashtag)
int get_Item_Ocorrencias(pItem item){
	return (*item).ocorrencias;
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	set_Item_Tag
//Funcionalidade:
//A funcao set_Item_Tag define o valor da componente <tag> de uma estrutura Item referenciada
//pelo ponteiro dado como primeiro argumento para o valor do ponteiro para char dado como segundo.
//Input:	ponteiro para Item e ponteiro para char
//Output:	---
void set_Item_Tag(pItem item, char *Hash_Text){
	(*item).tag = Hash_Text;
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	set_Item_Ocorrencias
//Funcionalidade:
//A funcao set_Item_Ocorrencias define o valor da componente <ocorrencias> de uma estrutura Item 
//referenciada pelo ponteiro dado como primeiro argumento para o valor inteiro dado como segundo.
//Input:	ponteiro para Item e inteiro
//Output:	---
void set_Item_Ocorrencias(pItem item, int new_Ocorrencias){
	(*item).ocorrencias = new_Ocorrencias;
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	cmp_Item
//Funcionalidade:
//A funcao cmp_Item compara duas estruturas Item referenciadas pelos ponteiros dados como argumento.
//Esta comparacao e baseada apenas no texto das hashtags das estruturas.
//Input:	ponteiros para Item
//Output:	=0 se forem iguais; !=0 se forem diferentes
int cmp_Item(pItem a, pItem b){
	if (a == ITEM_NONEXISTENT || b == ITEM_NONEXISTENT) return VALUE_NONEXISTENT;
	return strcoll(get_Item_Tag(a), get_Item_Tag(b));
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	print_Item
//Funcionalidade:
//A funcao print_Item imprime uma estrutura Item referenciada pelo ponteiro dado como argumento.
//Input:	ponteiro para Item
//Output:	---
void print_Item(pItem item){
	printf("%s %d\n", get_Item_Tag(item), get_Item_Ocorrencias(item));
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	free_Item
//Funcionalidade:
//A funcao free_Item liberta a memoria utilizada para uma estrutura Item referenciada pelo ponteiro
//dado como arguemento.
//Input:	ponteiro para Item
//Output:	---
void free_Item(pItem item){
	if (item == ITEM_NONEXISTENT) return;
	free((*item).tag);
	free(item);
}

////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////