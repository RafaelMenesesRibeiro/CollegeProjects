#include "HashPoint.h"

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	new_HashPoint
//Funcionalidade:
//A funcao new_HashPoint cria uma nova estrutura HashPoint. O ponteiro para Item da nova estrutura
//adquire o valor do ponteiro para Item dado como argumento. A componente <next> e inicializada a
//NULL.
//Input:	ponteiro para Item
//Output:	ponteiro para a nova estrutura HashPoint
pHashPoint new_HashPoint(pItem pNewItem){
	pHashPoint new = (pHashPoint) malloc(sizeof(struct HashPoint));
	set_HashPoint_Item(new, pNewItem);
	set_HashPoint_Next(new, HASHPOINT_INICIALIZATION);
	return new;
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	get_HashPoint_Item
//Funcionalidade:
//A funcao get_HashPoint_Item retorna a componente <item> de uma estrtura HashPoint referenciada
//pelo ponteiro dado como argumento.
//Input:	ponteiro para HashPoint
//Output:	ponteiro para Item
pItem get_HashPoint_Item(pHashPoint point){
	return (*point).item;
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	get_HashPoint_Next
//Funcionalidade:
//A funcao get_HashPoint_Next retorna a componente <next> de uma estrtura HashPoint referenciada
//pelo ponteiro dado como argumento.
//Input:	ponteiro para HashPoint
//Output:	ponteiro para HashPoint
pHashPoint get_HashPoint_Next(pHashPoint point){
	return (*point).next;
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	set_HashPoint_Item
//Funcionalidade:
//A funcao set_HashPoint_Item define o valor da componente <item> de uma estrutura HashPoint
//referenciada pelo ponteiro dado como primeiro argumento para o valor do ponteiro para Item 
//dado como segundo.
//Input:	ponteiro para HashPoint e ponteiro para Item
//Output:	---
void set_HashPoint_Item(pHashPoint point, pItem item){
	(*point).item = item;
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	set_HashPoint_Next
//Funcionalidade:
//A funcao set_HashPoint_Next define o valor da componente <next> de uma estrutura HashPoint
//referenciada pelo ponteiro dado como primeiro argumento para o valor do ponteiro para Item
//dado como segundo.
//Input:	ponteiro para HashPoint e ponteiro para Item
//Output:	---
void set_HashPoint_Next(pHashPoint point, pHashPoint next){
	(*point).next = next;
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	search_List_Item
//Funcionalidade:
//A funcao search_List_Item procura uma estrutura Item referenciado pelo ponteiro dado como segundo
//argumento na lista ligada cuja cabeca e referenciada pelo ponteiro dado como primeiro argumento.
//Input:	ponteiro para HashPoint e ponteiro para Item
//Output:	ponteiro para HashPoint (cuja componente <item> e igual a procurada)
pHashPoint search_List_Item(pHashPoint pHead, pItem searchItem){
	pHashPoint point;
	for (point = pHead; point != NULL; point = get_HashPoint_Next(point))
		if (cmp_Item(searchItem, get_HashPoint_Item(point)) == 0) return point;
	return ITEM_NONEXISTENT;
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	insert_List_Item
//Funcionalidade:
//A funcao insert_List_Item insere uma estrutura Item referenciada pelo ponteiro dado como segundo
//arguemento numa lista ligada cuja cabeca e referenciada pelo ponteiro dado como primeiro.
//Input:	ponteiro para HashPoint e ponteiro para Item
//Output:	ponteiro para HashPoint (cuja componente <item> e referencia a estrutura Item inserida)
pHashPoint insert_List_Item(pHashPoint pHead, pItem insertItem){
	pHashPoint new = new_HashPoint(insertItem);
	set_HashPoint_Next(new, pHead);
	return new;
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao:	free_HashPoint
//Funcionalidade:
//A funcao free_HashPoint liberta a memoria utilizada para uma estrutura HashPoint referenciada 
//pelo ponteiro dado como arguemento.
//Input:	ponteiro para HashPoint
//Output:	---
void free_HashPoint(pHashPoint point){
	if (point == HASHPOINT_NONEXISTENT) return;
	free_Item(get_HashPoint_Item(point));
	free(point);
}

////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////