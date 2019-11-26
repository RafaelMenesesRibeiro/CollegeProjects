#ifndef _proj2_
#define _proj2_

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <ctype.h>

#include "Item.h"
#include "HashPoint.h"
#include "DataBase.h"

void executa_A(pDataBase DataBase);
void executa_S(pDataBase DataBase);
void executa_M(pDataBase DataBase);
void executa_L(pDataBase DataBase);
void executa_X(pDataBase DataBase);

void split(char *line, pDataBase DataBase);
void hash_process(pDataBase DataBase, char *token);
int cmp_func(const void *a, const void *b);

int main();
# endif // proj2_h