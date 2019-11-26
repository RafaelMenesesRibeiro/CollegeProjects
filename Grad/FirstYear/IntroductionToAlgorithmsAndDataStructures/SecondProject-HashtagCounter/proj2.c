////////////////////////////////////////////////////////////////////////////////////////////////////
//Instituto Superior Tecnico
//2015/2016	2 semestre
//Introducao Algoritmos e Estruturas de Dados

//Rafael Ribeiro	84758
//Goncalo Castilho 	84722
////////////////////////////////////////////////////////////////////////////////////////////////////

#include "proj2.h"

#define TRUE 1

////////////////////////////////////////////////////////////////////////////////////////////////////
int main(){
	pDataBase DataBase = init_DataBase();
	while (TRUE){
		char command;
		command = getchar();
		switch (command){
			case 'a':
				executa_A(DataBase);
				break;	

			case 's':
				executa_S(DataBase);
				break;

 			case 'm':
            	executa_M(DataBase);
            	break;

            case 'l':
            	executa_L(DataBase);
            	break;

			case 'x':
				executa_X(DataBase);
				return EXIT_SUCCESS;
		}
	}
	return EXIT_FAILURE;
}