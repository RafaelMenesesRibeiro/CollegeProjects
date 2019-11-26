#ifndef _DataBase_
#define _DataBase_

#include "HashPoint.h"
#include "Item.h"

#define M 10007
#define DATABASE_NONEXISTENT NULL

//A estrutura DataBase representa todo o conhecimento sobre as hashtags inseridas no sistema.
//E constituido por um vetor de ponteiros para estrutura HashPoint (tabela de dispersao),
//dois inteiros; um que indica o numero de hashtags distintas e outro que indica a soma
//das ocorrencias de todas as hashtags distintas inseridas; e um ponteiro para Item
//que referencia a estrutura Item que contem o hashtag mais popular (em numero de ocorrencias)
//do sistema.
typedef struct DataBase{
	pHashPoint *Heads;
	int hash_Number;
	int hash_Total;
	pItem hash_MostPopular;
} *pDataBase;

////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcoes relacionadas com a estrutura DataBase

int hash(char *Hash_Text);
pDataBase init_DataBase();
pHashPoint* get_DataBase_Heads(pDataBase DataBase);
pHashPoint get_DataBaseList_Head(pDataBase DataBase, int i);
int get_DataBase_HashNumber(pDataBase DataBase);
int get_DataBase_HashTotal(pDataBase DataBase);
pItem get_DataBase_MostPopular(pDataBase DataBase);
void set_DataBase_Heads(pDataBase, pHashPoint *Data_Heads);
void set_DataBaseList_Head(pDataBase DataBase, int i, pHashPoint newHead);
void set_DataBase_HashNumber(pDataBase DataBase, int i);
void set_DataBase_HashTotal(pDataBase DataBase, int i);
void set_DataBase_MostPopular(pDataBase DataBase, pItem item);
pHashPoint search_HashTable_Item(pDataBase DataBase, pItem searchItem);
void insert_HashTable_Item(pDataBase DataBase, pItem insertItem);
void check_DataBase_MostPopular(pDataBase DataBase, pHashPoint point);
void traverse_HashTable(pDataBase DataBase, pItem *array);
int traverse_HashTableList(pItem *array, pHashPoint pHead, int j);
void free_HashList(pHashPoint pHead);
void free_HashTable(pDataBase DataBase);

#endif