%	Instituto Superior Tecnico
%	Logica para Programacao
%	1o Projeto, 2015/2016

%	Grupo 17
%	Goncalo Castilho 	- 84722
%	Rafael Ribeiro		- 84758

%-----------------------------------------------------------------------------------------------------------------------%
%-----------------------------------------------------------------------------------------------------------------------%
%															movs_possiveis/4											%			
%-----------------------------------------------------------------------------------------------------------------------%
%	movs_possiveis(Lab, Pos_atual, Movs, Poss).
%	Lab - Labirinto
%	Pos_atual - Posicao atual
%	Movs - Movimentos ja efetuados 
%	Poss - Movimentos Possiveis

%	Este predicado determina, dado um labirinto, uma posicao atual e os movimentos ja efetuados, os movimentos 
%	possiveis tendo em conta que cada posicao nao deve ser visitada mais do que uma vez.
%	Os movimentos possiveis sao apresentados pela ordem: c,b,e,d.
movs_possiveis(Lab, (L_atual, C_atual), Movs, Poss):- 
			obtem_imp(Lab, (L_atual, C_atual), Imp),
			obtem_movs(Imp, (L_atual, C_atual), Movs, [(c, -1, 0), (b, 1, 0), (e, 0, -1), (d, 0, 1)], Poss, []),
			!.

	%Encontra a lista com os movimentos impossiveis num certo ponto do Labirinto.
	obtem_imp(Lab, (L_atual, C_atual), Imp):- 
			nth1(L_atual, Lab, Lista_Linha),
			nth1(C_atual, Lista_Linha, Imp).
						
	%Calcula a posicao apos um movimento numa das 4 direcoes possiveis (se for valido).
	obtem_movs(_, _, _, [], Poss, Poss):- !.
	obtem_movs(Imp, (L_atual, C_atual), Movs, [(Direcao, Desloc_L, Desloc_C) | Outros_Movs], Poss, Movimentos_0):-
			not(member(Direcao, Imp)),
			Mov_L is L_atual + Desloc_L,
			Mov_C is C_atual + Desloc_C,
			not(member((_, Mov_L, Mov_C), Movs)),
			append(Movimentos_0, [(Direcao, Mov_L, Mov_C)], Movimentos_1),
			obtem_movs(Imp, (L_atual, C_atual), Movs, Outros_Movs, Poss, Movimentos_1),
			!.
	obtem_movs(Imp, (L_atual, C_atual), Movs, [_ | Outros_Movs], Poss, Movimentos_0):-
			obtem_movs(Imp, (L_atual, C_atual), Movs, Outros_Movs, Poss, Movimentos_0),
			!.
%-----------------------------------------------------------------------------------------------------------------------%
%-----------------------------------------------------------------------------------------------------------------------%
%															distancia/3													%																	
%-----------------------------------------------------------------------------------------------------------------------%
%	distancia((L1,C1), (L2,C2), Dist) 
%	L1 - Linha 1, 
%	C1 - Coluna 1, 
%	L2 - Linha 2, 
%	C2 - Coluna 2

%	Este predicado determina a distancia entre dois pontos, o ponto 1 e o ponto 2, dado as suas respetivas coordenadas.
distancia((L1, C1), (L2, C2), Dist):- Dist is abs(L1 - L2) + abs(C1 - C2).
%-----------------------------------------------------------------------------------------------------------------------%
%-----------------------------------------------------------------------------------------------------------------------%
%															ordena_poss/4												%
%-----------------------------------------------------------------------------------------------------------------------%
%	ordena_poss(Poss, Poss_ord, Pos_inicial, Pos_final).
%	Poss - Movimentos Possiveis
%	Poss_ord - Movimentos Ordenados
%	Pos_inicial - Posicao inicial
%	Pos_final - Posicao final 

%	Este predicado ordena os movimento possiveis segundo dois criterios: 
%	- movimentos conducentes a menor distancia a posicao final,
%	- em caso de empate, movimentos conducentes a maior distancia da posicao inicial.	
ordena_poss([P_0], [P_0], _, _):- !.
ordena_poss(Poss, Poss_Ord, Pos_inicial, Pos_final):-
			calcula_distancia(Poss, Pos_inicial, Pos_final, Distancias_f, []),
			sort(2, @=<, Distancias_f, Poss_Ord_D_Final),
			lista_repetidos(Poss_Ord_D_Final, Poss_Ord),
			!.

	%Determina a distancia entre cada um dos pontos e as posicoes inicial e final atraves do predicado distancia/3.
	calcula_distancia([], _, _, Distancias, Distancias):- !.
	calcula_distancia([(Poss_d, Poss_L, Poss_C) | Poss_Cauda], Pos_inicial, Pos_final, Distancias, L_d):- 
			distancia((Poss_L, Poss_C), Pos_final, Dist_f),
			distancia((Poss_L, Poss_C), Pos_inicial, Dist_i),
			append(L_d, [((Poss_d, Poss_L, Poss_C), Dist_f, Dist_i)], L_d_1),
			calcula_distancia(Poss_Cauda, Pos_inicial, Pos_final, Distancias, L_d_1),
			!.

	%Remove os dados nao necessarios para o resultado final.
	remove_excesso([], Lista_sem_excesso, Lista_sem_excesso):- !.
	remove_excesso([(P_0, _) | Excesso_Cauda], Lista_sem_excesso, Aux_0):-
			append(Aux_0, [P_0], Aux_1),
			remove_excesso(Excesso_Cauda, Lista_sem_excesso, Aux_1),
			!.

	%Ordena a lista de movimentos possiveis (ja ordenada por ordem decrescente da distancia ao Ponto Final) por ordem
	%crescente da distancia ao Ponto Inicial.
	%Caso em que so existem 2 pontos.
	lista_repetidos([(P_0, _, Dist_i_0), (P_1, _, Dist_i_1)], Lista_Ordenada):-
			sort(2, @>=, [(P_0, Dist_i_0), (P_1, Dist_i_1)], Lista_Ordenada_0),
			remove_excesso(Lista_Ordenada_0, Lista_Ordenada, []),
			!.
	%Caso em que existem 3 pontos e os dois primeiros sao equidistantes ao Ponto Final.
	lista_repetidos([(P_0, Dist_f_0, Dist_i_0), (P_1, Dist_f_0, Dist_i_1), (P_2, _, Dist_i_2)], Lista_Ordenada):-
			sort(2, @>=, [(P_0, Dist_i_0), (P_1, Dist_i_1)], Lista_Ordenada_0),
			append(Lista_Ordenada_0, [(P_2, Dist_i_2)], Lista_Ordenada_1),
			remove_excesso(Lista_Ordenada_1, Lista_Ordenada, []),
			!.
	%Caso em que existem 3 pontos mas nenhum par de pontos e equidistante ao Ponto Final.
	lista_repetidos([(P_0, Dist_f_0, _), (P_1, Dist_f_1, _), (P_2, Dist_f_2, [])], [P_0, P_1, P_2]):-
			Dist_f_0 \= Dist_f_1,
			Dist_f_1 \= Dist_f_2,
			!.
	%Caso em que existem 3 pontos e os dois ultimos sao equidistantes ao Ponto Final.
	lista_repetidos([(P_0, _, Dist_i_0), (P_1, Dist_f_1, Dist_i_1), (P_2, Dist_f_1, Dist_i_2)], Lista_Ordenada):-
			sort(2, @>=, [(P_1, Dist_i_1), (P_2, Dist_i_2)], Lista_Ordenada_0),
			append([(P_0, Dist_i_0)], Lista_Ordenada_0, Lista_Ordenada_1),
			remove_excesso(Lista_Ordenada_1, Lista_Ordenada, []),
			!.
%-----------------------------------------------------------------------------------------------------------------------%
%-----------------------------------------------------------------------------------------------------------------------%
%															resolve1/4													%
%-----------------------------------------------------------------------------------------------------------------------%
%	resolve1(Lab, Pos_inicial, Pos_final, Movs).
%	Lab - Labirinto
%	Pos_inicial - Posicao inicial
%	Pos_final - Posicao final
%	Movs - sequencia de movimentos

%	Este predicado devolve a sequencia de movimentos Movs correspondente a solucao para resolver o labirinto Lab, 
%	desde a posicao inicial (Pos_inicial) ate a posicao final (Pos_final).
% 	A solucao Movs obedece a duas condicoes: 
%	- nao passar mais do que uma vez pela mesma celula,
%	- os movimentos sao testados pela ordem: c,b,e,d.

%Se a posicao inicial for igual a posicao final o resolve1 devolve uma lista vazia que indica que nao foram efetuados 
%movimentos para chegar a posicao final.
resolve1(_, Pos_final,Pos_final, []):- !.

%O resolve1 chama um predicado auxiliar (que tem mais dois argumento, um que vai representar a posicao inicial e outro 
%que vai representar os movimentos efetuados ate a posicao atual) que vai resolver iterativamente o Labirinto ate 
%chegar a posicao final.
resolve1(Lab, Pos_inicial, Pos_final, Movs ):-  		
			resolve_aux(Lab, Pos_inicial, Pos_final, Pos_inicial, [(i, Pos_inicial)], Movs).                                      

	%Quando a resolve_aux chega a posicao final depois de iterar (posicao atual igual a posicao final), reverte a ordem 
	%dos elementos que foram inseridos por ordem inversa (movimento mais antigo fica a direita de um movimento mais 
	%recente).			
	resolve_aux(_, _, Pos_final, Pos_final, Movs_Inv, Movs):- 
			reverse(Movs_Inv, Movs), 
			!.
					
	%Se a Pos_atual do resolve_aux ainda nao for igual a Pos_final, o resolve_aux vai determinar os movimentos possiveis 
	%para a posicao atual tendo em conta a ordem c,b,e,d e que nunca se pode voltar a um posicao ja visitada. 
	%O resolve_aux vai chamar o resolve_aux com os novos argumentos (Pos_atual apos o novo movimento e Movs efetuados 
	%ate a posicao atual com o novo movimento).
	resolve_aux(Lab, Pos_inicial, Pos_final, Pos_atual, Movs_Ant, Movs):-
			movs_possiveis(Lab, Pos_atual, Movs_Ant, Poss),
			member((Dir, Pos_prox), Poss),
			resolve_aux(Lab, Pos_inicial, Pos_final, Pos_prox, [(Dir, Pos_prox) | Movs_Ant], Movs).
%-----------------------------------------------------------------------------------------------------------------------%
%-----------------------------------------------------------------------------------------------------------------------%
%															resolve2/4													%	
%-----------------------------------------------------------------------------------------------------------------------%
%	resolve2(Lab, Pos_inicial, Pos_final, Movs).
%	Lab - Labirinto
%	Pos_inicial - Posicao inicial
%	Pos_final - Posicao final
%	Movs - sequencia de movimentos

%	Este predicado devolve a sequencia de movimentos Movs correspondente a solucao para resolver o labirinto Lab, 
%	desde a posicao inicial (Pos_inicial) ate a posicao final (Pos_final).
% 	A solucao Movs obedece a duas condicoes:
%	- o movimento efetuado esta a menor distancia da posicao final,
%	- em caso de empate, o movimento efetuado esta a maior distancia da posicao inicial,
%	- em caso de um segundo empate os movimentos sao testados pela ordem c,b,e,d.

%Se a posicao inicial for igual a posicao final o resolve2 devolve uma lista vazia que indica que nao foram efetuados 
%movimentos para chegar a posicao final.
resolve2(_, Pos_final,Pos_final, []):- !.

%O resolve2 chama um predicado auxiliar (que tem mais dois argumento, um que vai representar a posicao inicial e outro 
%que vai representar os movimentos efetuados ate a posicao atual) que vai resolver iterativamente o Labirinto ate 
%chegar a posicao final.
resolve2(Lab, Pos_inicial, Pos_final, Movs ):-	
			resolve2_aux(Lab, Pos_inicial, Pos_final, Pos_inicial, [(i, Pos_inicial)], Movs),
			!.   
						
	%Quando a resolve2_aux chega a posicao final depois de iterar (posicao atual igual a posicao final), reverte a ordem 
	%dos elementos que foram inseridos por ordem inversa (movimento mais antigo fica a direita de um movimento mais 
	%recente).		
	resolve2_aux(_, _, Pos_final, Pos_final, Movs_Inv, Movs):- 
			reverse(Movs_Inv, Movs), 
			!.

	%Se a Pos_atual do resolve2_aux ainda nao for igual a Pos_final, o resolve2_aux vai determinar os movimentos 
	%possiveis para a posicao atual tendo em conta que nunca se pode voltar a uma posicao ja visitada e comecando pelo 
	%movimento que leva a posicao menos distante da posicao final. Se houver empate, comeca pelo movimento que leva a 
	%posicao mais distante da posicao inicial (de entre os empatados). Chama o resolve2_aux com os novos argumentos 
	%(Pos_atual apos o novo movimento e Movs efetuados ate a posicao atual com o novo movimento).						
	resolve2_aux(Lab, Pos_inicial, Pos_final, Pos_atual, Movs_Ant, Movs):- 
			movs_possiveis(Lab, Pos_atual, Movs_Ant, Poss),
			ordena_poss(Poss, Poss_Ord, Pos_inicial, Pos_final),
			member((Dir, Pos_prox), Poss_Ord),
			resolve2_aux(Lab, Pos_inicial, Pos_final, Pos_prox, [(Dir, Pos_prox) | Movs_Ant], Movs).
%-----------------------------------------------------------------------------------------------------------------------%
%-----------------------------------------------------------------------------------------------------------------------%						
%															FIM DE CODIGO												%
%-----------------------------------------------------------------------------------------------------------------------%