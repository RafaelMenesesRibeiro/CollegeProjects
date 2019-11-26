#ifndef _Item_
#define _Item_

#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#define ITEM_NONEXISTENT NULL
#define ITEM_INICIALIZATION 1
#define VALUE_NONEXISTENT -1

//A estrutura Hash_Struct e constituida por um ponteiro "Tag" que referencia uma string 
//(texto da hashtag) e por um inteiro "Ocorrencias" que representa o numero de hashtags iguais 
//utilizados.
typedef struct Item{
	char *tag;
	int ocorrencias;
} *pItem;

////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcoes relacionadas com a estrutra Item

pItem new_Item(char *Hash_Text);
char* get_Item_Tag(pItem item);
int get_Item_Ocorrencias(pItem item);
void set_Item_Tag(pItem item, char *Hash_Text);
void set_Item_Ocorrencias(pItem item, int new_Ocorrencias);
int cmp_Item(pItem a, pItem b);
void print_Item(pItem item);
void free_Item(pItem item);

#endif