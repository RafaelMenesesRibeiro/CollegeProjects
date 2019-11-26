#ifndef _HashPoint_
#define _HashPoint_

#include "Item.h"

#define HASHPOINT_NONEXISTENT NULL
#define HASHPOINT_INICIALIZATION NULL

//A estrutura HashPoint representa um no de uma lista ligada. E constituido por um ponteiro para
//uma estrutura Item e por um ponteiro para uma estrutura HashPoint (proximo no da lista ligada).
typedef struct HashPoint{
	pItem item;
	struct HashPoint *next;
} *pHashPoint;

////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcoes relacionadas com a estrutura HashPoint

pHashPoint new_HashPoint(pItem pNewItem);
pItem get_HashPoint_Item(pHashPoint point);
pHashPoint get_HashPoint_Next(pHashPoint point);
void set_HashPoint_Item(pHashPoint, pItem item);
void set_HashPoint_Next(pHashPoint point, pHashPoint next);
pHashPoint search_List_Item(pHashPoint pHead, pItem searchItem);
pHashPoint insert_List_Item(pHashPoint pHead, pItem insertItem);
void free_HashPoint(pHashPoint point);

#endif