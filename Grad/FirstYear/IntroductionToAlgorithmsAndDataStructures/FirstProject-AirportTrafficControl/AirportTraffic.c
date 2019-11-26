//Rafael Ribeiro, Goncalo Castilho.
//IST, Introducao Algortimos e Estruturas Dados
//2015/2016 2 Semestre

/*                Definicao do PROGRAMA                        */
//Este programa  e responsavel por gerir e manipular informacao
//referente a gestao de uma rede de aeroportos.


/*                Definicao das SIGLAS                      */
//Define-se:
//id - string de 3 letras que identifica um aeroporto
//capacidade - (>0) que define o numero maximo de voos que incluem o aeroporto
//estado - define se o aeroporto esta aberto ou fechado
//Nvoos - numero de voos
//Naeroportos - numero total de aeroportos
//Noutgoing - numero de voos que partem de um aeroporto
//Nincoming - numero de voos que chegam a um aeroporto


/*                Definicao das FUNCOES PRINCIPAIS             */
//Funcao: Executa_A
//Objetivo: Adiciona um novo aeroporto (id) a rede.
//Input: A id capacidade

//Funcao: Executa_I
//Objetivo: Altera a capacidade de um aeroporto existente (id).
//Input: I id capacidade

//Funcao: Executa_F
//Objetivo: Adiciona um voo de ida e volta (entre id_1 e id_2).
//Input: F id_1 id_2

//Funcao: Executa_G
//Objetivo: Adiciona uma voo de ida (de id_1 para id_2).
//Input: G id_1 id_2

//Funcao: Executa_R
//Objetivo: Remove voo de ida (de id_1 para id_2).
//Input: R id_1 id_2

//Funcao: Executa_S
//Objetivo: Remove voo de ida e volta (entre id_1 e id_2).
//Input: S id_1 id_2

//Funcao: Executa_N
//Objetivo: Indica o numero de voos (entre id_1 e id_2).
//Input: N id_1 id_2

//Funcao: Executa_P
//Objetivo: Indica o aeroporto com maior numero de voos.
//Input: P
//Output: Aeroporto com mais rotas id:Noutgoing:Nincoming

//Funcao: Executa_Q
//Objetivo: Indica o aeroporto mais conectado (a outros aeroportos).
//Input: Q
//Output: Aeroporto com mais ligacoes id:Naeroportos

//Funcao: Executa_V
//Objetivo: Indica o voo mais popular.
//Input: V
//Output: Voo mais popular id1:id2:Nvoos

//Funcao: Executa_C
//Objetivo: Encerra aeroporto (id).
//Input: C id

//Funcao: Executa_O
//Objetivo: Reabre aeroporto (id).
//Input: O id

//Funcao: Executa_L
//Objetivo: Emite listagem:
//tipo 0: ordem de entrada no sistema, 
//tipo 1: ordem lexicografica, 
//tipo 2: histograma d(k) do numero de aeroportos com k voos).
//Input: L tipo
//Output 0: id:capacidade:Noutgoing:Nincoming
//Output 1: id:capacidade:Noutgoing:Nincoming
//Output 2: k':d(k')
//       k'':d(k'')     , com k crescente maior que 0

//Funcao: Executa_X
//Objetivo: Sair do programa
//Input: X
//Output: Nvoos:Naeroportos

/*                Definicao das FUNCOES AUXILIARES             */
//Funcao: LeID
//Objetivo: Le o ID introduzido no terminal.
//Output: string ID

//Funcao: LeValor
//Objetivo: Le o valor introduzido no terminal.
//Output: int valor

//Funcao: Encontra_Aeroporto
//Objetivo: Encontra a posicao do aeroporto especificado no vetor de aeroportos.
//Output: int posicao

//Funcao: Maior_numero_voos
//Objetivo: Encontra a posicao do aeroporto com mais voos no vetor de aeroportos.
//Output: int posicao

//Funcao: Insertion
//Objetivo: Ordena o vetor de aeroportos especificado por ordem lexicografica de ID.

//Funcao: Aeroporto_Menor
//Objetivo: Indica qual o ID menor (entre o ID de dois aeroportos).
//Output: int valor

//Funcao: Duplica_Estrutura
//Objetivo: Duplica uma estrutura de aeroportos

/*                         PROGRAMA                      */

#include <stdio.h>
#include <string.h>

#define MAX_AEROPORTOS 1000
#define MAX_IDENTIFICADOR 3 + 1
#define AER_INEXT -1


typedef struct {
   char identificador[MAX_IDENTIFICADOR];
   int capacidade;
   int estado;
   int voos_entrada;
   int voos_saida;
} Aeroporto;

Aeroporto aeroportos [MAX_AEROPORTOS];
int matriz[MAX_AEROPORTOS][MAX_AEROPORTOS];
int num_aero;
char id1[MAX_IDENTIFICADOR];
char id2[MAX_IDENTIFICADOR];

void Executa_A();
void Executa_I();
void Executa_F();
void Executa_G();
void Executa_R();
void Executa_S();
void Executa_N();
void Executa_P();
void Executa_Q();
void Executa_V();
void Executa_C();
void Executa_O();
void Executa_L();
void Executa_X();

void LeID(char id[]);
int LeValor();
int Encontra_Aeroporto(char id[]);
int Maior_numero_voos();
void Insertion(Aeroporto aeroportos[], int num_aero);
int Aeroporto_Menor(Aeroporto aux, Aeroporto j);
void Duplica_Estrutura(Aeroporto Nova_Estrutura[]);


int main(){
   char comand;
   int i = 0, j = 0;
   num_aero = 0;
   
   for (i = 0; i < MAX_AEROPORTOS; ++i){
      for (j = 0; j < MAX_AEROPORTOS; ++j){
         matriz[i][j] = 0;
      }
   }

   while (1){
      comand = getchar();
      switch (comand){
         case 'A':
            Executa_A();
            break;
         
         case 'I':
            Executa_I();
            break;
         
         case 'F':
            Executa_F();
            break;
         
         case 'G':
            Executa_G();
            break;
         
         case 'R':
            Executa_R();
            break;
         
         case 'S':
            Executa_S();
            break;
         
         case 'N':
            Executa_N();
            break;
         
         case 'P':
            Executa_P();
            break;
         
         case 'Q':
            Executa_Q();
            break;
         
         case 'V':
            Executa_V();
            break;
         
         case 'C':
            Executa_C();
            break;
         
         case 'O':
            Executa_O();
            break;
         
         case 'L':
            Executa_L();
            break;
         
         case 'X':
            Executa_X();
            return 1;
      }
   }
   return 0;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao: Executa_A
//Objetivo: Adiciona um novo aeroporto (id) a rede.
//Input: A id capacidade
void Executa_A(){
   int v = 0;
   
   //Le a identificacao do aeroporto.
   LeID(id1);

   //Le o valor da capacidade do aeroporto especificado. Se esse valor nao for
   //um inteiro maior que 0, retorna sem criar o aeroporto.
   v = LeValor();
   if (v < 0){
      return;
   }

   //Se ambos os dados introduzidos forem validos, e criado um aeroporto
   //com a identificacao e a capacidade especificadas. O estado e iniciado
   //a 1 (ativo).
   strcpy(aeroportos[num_aero].identificador, id1);
   aeroportos[num_aero].capacidade = v;
   aeroportos[num_aero].estado = 1;
   aeroportos[num_aero].voos_entrada = 0;
   aeroportos[num_aero].voos_saida = 0;

   num_aero += 1;
   return;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao: Executa_I
//Objetivo: Altera a capacidade de um aeroporto existente (id).
//Input: I id capacidade
void Executa_I(){
   int i = 0, nova_capacidade = 0;
   
   //Le o identificador do aeroporto.
   LeID(id1);

   //Procura a posicao do aeroporto no vetor de aeroportos. Se o aeroporto nao existir
   //ou estiver encerrado, retorna sem efetuar alteracoes.
   i = Encontra_Aeroporto(id1);
   if (i == AER_INEXT || aeroportos[i].estado == 0){
      printf("*Capacidade de %s inalterada\n", id1);
      return;
   }

   //Le o valor de alteracao da capacidade do aeroporto especificado. Se esse valor
   //for inferior ao numero de voos totais atuais ou invalido, retorna sem efetuar
   //alteracoes.
   nova_capacidade = aeroportos[i].capacidade + LeValor();
   if (nova_capacidade < aeroportos[i].voos_entrada + aeroportos[i].voos_saida){
      printf("*Capacidade de %s inalterada\n", id1);
      return;
   }

   //Se ambos os dados introduzidos forem validos, a capacidade do aeroporto 
   //especificado e alterada.
   aeroportos[i].capacidade = nova_capacidade;
   return;
}  

////////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao: Executa_F
//Objetivo: Adiciona um voo de ida e volta (entre id_1 e id_2).
//Input: F id_1 id_2
void Executa_F(){
   int i1 = 0, i2 = 0;

   //Le os identificadores dos aeroportos.
   LeID(id1);
   LeID(id2);

   //Procura a posicao dos aeroportos no vetor de aeroportos. Se algum dos aeroportos
   //nao existir ou estiver encerrado, retorna sem efetuar alteracoes.
   i1 = Encontra_Aeroporto(id1);
   i2 = Encontra_Aeroporto(id2);
   if ((i1 == AER_INEXT || aeroportos[i1].estado == 0) || (i2 == AER_INEXT || aeroportos[i2].estado == 0) 
      || (aeroportos[i1].capacidade < aeroportos[i1].voos_entrada + aeroportos[i1].voos_saida + 2)
      || (aeroportos[i2].capacidade < aeroportos[i2].voos_entrada + aeroportos[i2].voos_saida + 2)){
      printf("*Impossivel adicionar voo RT %s %s\n", id1, id2);
      return;
   }
   
   //Se ambos os dados inseridos forem validos, adiciona os voos na estrutura de dados 
   //referente aos aeroportos especificados e na matriz dos voos.
   aeroportos[i1].voos_entrada += 1;
   aeroportos[i1].voos_saida += 1;
   aeroportos[i2].voos_entrada += 1;
   aeroportos[i2].voos_saida += 1;

   matriz[i1][i2] += 1;
   matriz[i2][i1] += 1;
   return;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao: Executa_G
//Objetivo: Adiciona uma voo de ida (de id_1 para id_2).
//Input: G id_1 id_2
void Executa_G(){
   int i1 = 0, i2 = 0;

   //Le os identificadores dos aeroportos.
   LeID(id1);
   LeID(id2);

   //Procura a posicao dos aeroportos no vetor de aeroportos. Se algum dos aeroportos
   //nao existir ou estiver encerrado, retorna sem efetuar alteracoes.
   i1 = Encontra_Aeroporto(id1);
   i2 = Encontra_Aeroporto(id2);
   if ((i1 == AER_INEXT || aeroportos[i1].estado == 0) || (i2 == AER_INEXT || aeroportos[i2].estado == 0) 
      || (aeroportos[i1].capacidade < aeroportos[i1].voos_entrada + aeroportos[i1].voos_saida + 1)
      || (aeroportos[i2].capacidade < aeroportos[i2].voos_entrada + aeroportos[i2].voos_saida + 1)){
      printf("*Impossivel adicionar voo %s %s\n", id1, id2);
      return;
   }
   
   //Se ambos os dados inseridos forem validos, adiciona o voo na estrutura de dados 
   //referente aos aeroportos especificados e na matriz dos voos.
   aeroportos[i1].voos_saida += 1;
   aeroportos[i2].voos_entrada += 1;

   matriz[i1][i2] += 1;
   return;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao: Executa_R
//Objetivo: Remove voo de ida (de id_1 para id_2).
//Input: R id_1 id_2
void Executa_R(){
   int i1 = 0, i2 = 0;

   //Le os identificadores dos aeroportos.
   LeID(id1);
   LeID(id2);

   //Procura a posicao dos aeroportos no vetor de aeroportos. Se algum dos aeroportos
   //nao existir ou estiver encerrado, retorna sem efetuar alteracoes.
   i1 = Encontra_Aeroporto(id1);
   i2 = Encontra_Aeroporto(id2);
   if ((i1 == AER_INEXT || aeroportos[i1].estado == 0) || (i2 == AER_INEXT || aeroportos[i2].estado == 0) 
      || matriz[i1][i2] == 0){
      printf("*Impossivel remover voo %s %s\n", id1, id2);
      return;
   }
   
   //Se ambos os dados inseridos forem validos, subtrai o voo na estrutura de dados 
   //referente aos aeroportos especificados e na matriz dos voos.
   aeroportos[i1].voos_saida -= 1;
   aeroportos[i2].voos_entrada -= 1;

   matriz[i1][i2] -= 1;
   return;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao: Executa_S
//Objetivo: Remove voo de ida e volta (entre id_1 e id_2).
//Input: S id_1 id_2
void Executa_S(){
   int i1 = 0, i2 = 0;

   //Le os identificadores dos aeroportos.
   LeID(id1);
   LeID(id2);

   //Procura a posicao dos aeroportos no vetor de aeroportos. Se algum dos aeroportos
   //nao existir ou estiver encerrado, retorna sem efetuar alteracoes.
   i1 = Encontra_Aeroporto(id1);
   i2 = Encontra_Aeroporto(id2);
   if ((i1 == AER_INEXT || aeroportos[i1].estado == 0) || (i2 == AER_INEXT || aeroportos[i2].estado == 0) 
      || matriz[i1][i2] == 0 || matriz[i2][i1] == 0){
      printf("*Impossivel remover voo RT %s %s\n", id1, id2);
      return;
   }

   //Se ambos os dados inseridos forem validos, subtrai os voos na estrutura de dados 
   //referente aos aeroportos especificados e na matriz dos voos.
   aeroportos[i1].voos_saida -= 1;
   aeroportos[i1].voos_entrada -= 1;
   aeroportos[i2].voos_saida -= 1;
   aeroportos[i2].voos_entrada -= 1;

   matriz[i1][i2] -= 1;
   matriz[i2][i1] -= 1;
   return;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao: Executa_N
//Objetivo: indica o numero de voos (entre id_1 e id_2).
//Input: N id_1 id_2
void Executa_N(){
   int i1 = 0, i2 = 0;

   //Le os identificadores dos aeroportos.
   LeID(id1);
   LeID(id2);

   //Procura a posicao dos aeroportos no vetor de aeroportos. Se algum dos aeroportos
   //nao existir ou estiver encerrado, retorna sem efetuar alteracoes.
   i1 = Encontra_Aeroporto(id1);
   i2 = Encontra_Aeroporto(id2);
   if (i1 == AER_INEXT){ 
      printf("*Aeroporto %s inexistente\n", id1);
      return;
   }
   if  (i2 == AER_INEXT){
      printf("*Aeroporto %s inexistente\n", id2);
      return;
   }

   //Se ambos os dados inseridos forem validos, retorna o numero de voos de id1 para
   //id2 e de id2 para id1.
   printf("Voos entre cidades %s:%s:%d:%d\n", id1, id2, matriz[i1][i2], matriz[i2][i1]);
   return;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao: Executa_P
//Objetivo: Indica o aeroporto com maior numero de voos.
//Input: P
//Output: Aeroporto com mais rotas id:Noutgoing:Nincoming
void Executa_P(){
   int maxi = 0;
   maxi = Maior_numero_voos();

   //Retorna o identificador, o numero de voos que partem e o numero de voos que 
   //chegam ao aeroporto com maior numero de voos.
   printf("Aeroporto com mais rotas %s:%d:%d\n", aeroportos[maxi].identificador, 
      aeroportos[maxi].voos_saida, aeroportos[maxi].voos_entrada);
   return;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao: Executa_Q
//Objetivo: Indica o aeroporto mais conectado (a outros aeroportos).
//Input: Q
//Output: Aeroporto com mais ligacoes id:Naeroportos
void Executa_Q(){
   int i = 0, j = 0;
   int num_coneccoes = 0;
   int max = 0, maxi = 0;

   //Varre a matriz e verifica se o par de aeroportos (i, j) tem algum voo (de i
   //para j ou de j para i). Se for o caso, adiciona ao numero de coneccoes do
   // aeroporto i.
   for (i = 0; i < num_aero; i++){
      for (j = 0; j < num_aero; j++){
         if ((matriz[i][j] > 0) || (matriz[j][i] > 0)){
            num_coneccoes += 1;
         }
      }
      if (num_coneccoes > max){
         max = num_coneccoes;
         maxi = i;
      }
      num_coneccoes = 0;
   }

   //Retorna o identificador e o numero de coneccoes do aeroporto mais conectado.
   printf("Aeroporto com mais ligacoes %s:%d\n", aeroportos[maxi].identificador, max);
   return;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao: Executa_V
//Objetivo: Indica o voo mais popular.
//Input: V
//Output: Voo mais popular id1:id2:Nvoos
void Executa_V(){
   int i = 0, j = 0, c = 0;
   int max = 0;
   int maxi = 0, maxj = 0;

   //Por cada par de aeroportos, calcula qual o voo mais popular. Retorna os
   //identificadores do aeroporto de saida (maxi), de entrada (maxj)
   //e o numero de voos a sair de maxi para maxj.
   for (i = 0; i < num_aero; i++){
      for (j = 0; j < num_aero; j++){
         if ((c = matriz[i][j]) > max){
            max = c;
            maxi = i;
            maxj = j;
         }
      }
   }
   printf("Voo mais popular %s:%s:%d\n", aeroportos[maxi].identificador, 
      aeroportos[maxj].identificador, max);
   return;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao: Executa_C
//Objetivo: Encerra aeroporto (id).
//Input: C id
void Executa_C(){
   int i = 0, j = 0, c = 0;

   //Le o identificador do aeroporto.
   LeID(id1);

   //Procura a posicao do aeroporto no vetor de aeroportos. Se este nao existir, 
   //retorna sem efetuar alteracoes.
   i = Encontra_Aeroporto(id1);
   if (i == AER_INEXT){
      printf("*Aeroporto %s inexistente\n", id1);
      return;
   }
   
   //Se os dados inseridos forem validos, encerra o aeroporto e remove todos os voos
   //com partida ou chegada no aeroporto especificado.
   aeroportos[i].voos_saida = 0;
   aeroportos[i].voos_entrada = 0;

   for (j = 0; j < num_aero; j++){
      c = matriz[i][j];
      if (c > 0){
         aeroportos[j].voos_entrada -= c;
         matriz[i][j] = 0;
      }

      c = matriz[j][i];
      if (c > 0){
         aeroportos[j].voos_saida -= c;
         matriz[j][i] = 0;
      }
   }
   aeroportos[i].estado = 0;
   return;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao: Executa_O
//Objetivo: Reabre aeroporto (id).
//Input: O id
void Executa_O(){
   int i = 0;

   //Le o identificador do aeroporto.
   LeID(id1);

   //Procura a posicao do aeroporto no vetor de aeroportos. Se este nao existir ou 
   //retorna sem efetuar alteracoes.
   i = Encontra_Aeroporto(id1);
   if (i == AER_INEXT){
      printf("*Aeroporto %s inexistente\n", id1);
      return;
   }

   //Altera o estado do aeroporto especificado no vetor de aeroportos para ativo.
   aeroportos[i].estado = 1;
   return;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao: Executa_L
//Objetivo: Emite listagem:
//tipo 0: ordem de entrada no sistema, 
//tipo 1: ordem lexicografica, 
//tipo 2: histograma d(k) do numero de aeroportos com k voos).
//Input: L tipo
//Output 0: id:capacidade:Noutgoing:Nincoming
//Output 1: id:capacidade:Noutgoing:Nincoming
//Output 2: k':d(k')
//       k'':d(k'')     , com k crescente maior que 0
void Executa_L(){
   int c = 0, t = 0;
   int i = 0, j = 0;
   int max = 0, maxi = 0;
   Aeroporto Nova_Estrutura [MAX_AEROPORTOS];

   c = LeValor();
   switch (c){
      case 0:
         //Imprime os aeroportos existentes por ordem de entrada no sistema.
         //Por cada aeroporto, identificador:capacidade:voos.saida:voos.entrada
         for (i = 0; i < num_aero; i++){
            printf("%s:%d:%d:%d\n", 
               aeroportos[i].identificador, 
               aeroportos[i].capacidade,
               aeroportos[i].voos_saida,
               aeroportos[i].voos_entrada);
         }
         break;

      case 1:
         //Duplica o vetor de aeroportos para nao alterar o vetor original,
         //que pode ser necessario no futuro (especificamente, a ordem dos
         //aeroportos; a ordem de entrada no sistema).
         Duplica_Estrutura(Nova_Estrutura);

         //Ordena o vetor por ordem lexicografica dos identificadores dos
         //aeroportos.
         //Por cada aeroporto, identificador:capacidade:voos.saida:voos.entrada
         
         Insertion(Nova_Estrutura, num_aero);
         for (i = 0; i < num_aero; i++){
            printf("%s:%d:%d:%d\n", 
               Nova_Estrutura[i].identificador, 
               Nova_Estrutura[i].capacidade,
               Nova_Estrutura[i].voos_saida,
               Nova_Estrutura[i].voos_entrada);
         }
         break;

      case 2:
         //Calcula o aeroporto com mais voos totais, e calcula esse valor.
         maxi = Maior_numero_voos();
         max = aeroportos[maxi].voos_saida + aeroportos[maxi].voos_entrada;

         //Varre o vetor aeroportos para calcular quantos aeroportos
         //tem exetamente i voos, (com i de 0 ao valor maximo calculado em cima).
         //Imprime para cada i, o numero de aeroportos (t) cujos voos totais adicionam i,
         //se existir pelo menos um t.
         for (i = 0; i <= max; i++){
            for (j = 0; j < num_aero; j++){
               if (aeroportos[j].voos_saida + aeroportos[j].voos_entrada == i){
                  t += 1;
               }
            }
            if (t > 0){
               printf("%d:%d\n", i, t);
               t = 0;
            }
         }
         break;
   }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao: Executa_X
//Objetivo: Sair do programa
//Input: X
//Output: Nvoos:Naeroportos
void Executa_X(){
   int i = 0;
   int total = 0;

   //Soma todos os voos existentes na matriz de voos.
   for (i = 0; i < num_aero; i++){
      total += aeroportos[i].voos_saida;
   }
   printf("%d:%d\n", total, num_aero);
   return;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////
/*                                        FUNCOES AUXILIARES                                          */
////////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao: LeID
//Objetivo: Le o ID introduzido no terminal.
//Output: string ID
void LeID(char id[]){
   //Le o identificador do aeroporto.
   scanf("%s", id);
   return;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao: LeValor
//Objetivo: Le o valor introduzido no terminal.
//Output: int valor
int LeValor(){
   int v = 0;

   //Le o valor introduzido.
   scanf("%d", &v);
   return v;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao: Encontra_Aeroporto
//Objetivo: Encontra a posicao do aeroporto especificado no vetor de aeroportos.
//Output: int posicao
int Encontra_Aeroporto(char id[]){
   int i = 0;

   //Encontra a posicao do aeroporto especificado no vetor aeroportos.
   for (i = 0; i < num_aero; i++){
      if (strcmp(aeroportos[i].identificador, id) == 0){
         return i;
      }
   }

   //Se o aeroporto nao estiver definido retorna -1 (indicando que o aeroporto
   //nao esta definido).
   return -1;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao: Maior_numero_voos
//Objetivo: Encontra a posicao do aeroporto com mais voos no vetor de aeroportos.
//Output: int posicao
int Maior_numero_voos(){
   int i = 0, c = 0;
   int max = aeroportos[0].voos_entrada + aeroportos[0].voos_saida;
   int maxi = 0;

   //Procura a posicao do aeroporto com maior numero de voos.
   for (i = 1; i < num_aero; i++){
      if ((c = aeroportos[i].voos_entrada + aeroportos[i].voos_saida) > max){
         max = c;
         maxi = i;
      }
   }
   return maxi;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao: Insertion
//Objetivo: Ordena o vetor de aeroportos especificado por ordem lexicografica de ID.
void Insertion(Aeroporto aeroportos[], int num_aero){
   int i  = 0, j = 0;

   //Reordena o vetor por ordem lexicografica dos identificadores dos
   //aeroportos existentes.
   for (i = 1; i < num_aero; i++){
      Aeroporto aux = aeroportos[i];
      j = i - 1;
      while(j >= 0 && (Aeroporto_Menor(aux, aeroportos[j]) < 0)){
         aeroportos[j + 1] = aeroportos[j];
         j -= 1;
      }
      aeroportos[j + 1] = aux;
   }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao: Aeroporto_Menor
//Objetivo: Indica qual o ID menor (entre o ID de dois aeroportos).
//Output: int valor
int Aeroporto_Menor(Aeroporto aux, Aeroporto j){
   strcpy(id1, aux.identificador);
   strcpy(id2, j.identificador);

   //Retorna um valor negativo se id1 for "menor" que id2, um valor positivo
   //se id2 for "menor" que id1 ou retorna 0 se forem iguais.
   return strcoll(id1, id2);
}

////////////////////////////////////////////////////////////////////////////////////////////////////////
//Funcao: Duplica_Estrutura
//Objetivo: Duplica uma estrutura de aeroportos
void Duplica_Estrutura(Aeroporto Nova_Estrutura[]){
   int i = 0;

   //Copia todos os aeroportos e respetivos dados para a nova estrutura
   //passada no argumento.
   for (i = 0; i < num_aero; i++){
      strcpy(Nova_Estrutura[i].identificador, aeroportos[i].identificador);
      Nova_Estrutura[i].capacidade = aeroportos[i].capacidade;
      Nova_Estrutura[i].estado = aeroportos[i].estado;
      Nova_Estrutura[i].voos_entrada = aeroportos[i].voos_entrada;
      Nova_Estrutura[i].voos_saida = aeroportos[i].voos_saida;
   }
}
////////////////////////////////////////////////////////////////////////////////////////////////////////