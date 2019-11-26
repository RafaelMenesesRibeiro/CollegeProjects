# Francisco Teixeira de Barros 85069, Rafael M. Lucas Ribeiro 84758, TP tg024

# -----------------------------------------------------------------------------#
#                             TAD Coordenada HELP			       #
# -----------------------------------------------------------------------------#
'''	cria_coordenada(l, c): 
	Construtor - recebe dois inteiros positivos e cria uma coordenada xy. '''

'''	coordenada_linha(xy): 
	Seletor - se for coordenada, devolve o valor da abcissa (linha). '''

'''	e_coordenada(u): 
	Reconhecedor - verifica se o argumento universal e uma coordenada e retorna
	um valor logico. '''

'''	coordenadas_iguais(xy1,xy2): 
	Teste - retorna um valor logico dependendo da igualdade dos argumentos. '''

'''	coordenada_para_cadeia(xy): 
	Transformador - se o argumento for coordenada, representa-a na forma
	'(l, c)'. '''

# -----------------------------------------------------------------------------#
#                            TAD Coordenada CODIGO                             #
# -----------------------------------------------------------------------------#

def cria_coordenada(l, c):
    if not (isinstance(l,(int)) and l>0 and isinstance(c,(int)) and c>0):
            raise ValueError ("cria_coordenada: argumentos invalidos")
    return (l,c)

def coordenada_linha(xy):
    if e_coordenada(xy):
        return xy[0]
    return False

def coordenada_coluna(xy):
    if e_coordenada(xy):
        return xy[1]
    return False

def e_coordenada(u):
    if isinstance(u, (tuple)):
        if len(u)==2:
            return (isinstance(u[0],int) and u[0]>0\
                    and isinstance(u[1],(int)) and u[1]>0)
    return False

def coordenadas_iguais(xy1,xy2):
    if not (e_coordenada(xy1) and e_coordenada(xy2)):
        raise ValueError ('coordenadas_iguais: entradas tem de ser coordenadas')
    if not (coordenada_linha(xy1) - coordenada_linha(xy2) == 0 and\
            coordenada_coluna(xy1) - coordenada_coluna(xy2) == 0):
        return False
    return True

def coordenada_para_cadeia(xy):
    if not e_coordenada(xy):
        raise ValueError\
              ('coordenada_para_cadeia: entrada tem de ser coordenada')
    return ('(' +  str((coordenada_linha(xy))) + ' : '\
            + str((coordenada_coluna(xy))) + ')')

# -----------------------------------------------------------------------------#
#                              TAD Tabuleiro HELP                              #
# -----------------------------------------------------------------------------#
'''	cria_tabuleiro(t): 
	Construtor - recebe tuplo de tuplos que correspondem as	especificacoes do 
	tabuleiro e devolve a representacao interna de acordo com o 
	seguinte esquema: especificacoes, dicionario('coordenada': valor da celula).'''

'''	tabuleiro_dimensoes(t): 
	Seletor - se o argumento for tabuleiro, devolve um tuplo com as dimensoes
	do tabuleiro, tendo por base as especificacoes.'''

'''	tabuleiro_especificacoes(t): 
	Seletor - se o argumento for tabuleiro, devolve a primeira posicao
	do tuplo t, correspondente as especificacoes. '''

'''	tabuleiro_celula(t, xy): 
	Seletor - se receber como argumentos um tabuleiro (t) e uma coordenada (xy),
	devolve o valor da celula de t indicada por xy. O valor retornado pertence
	a {0, 1, 2}, indicando se a celula esta preenchida, vazia ou indefinida. '''

'''	tabuleiro_preenche_celula(t, xy, v): 
	Transformador - se receber como argumentos um tabuleiro, uma coordenada 
	e um valor pertencente a {0, 1, 2}, modifica o valor da celula para 
	o valor indicado. '''  

'''	e_tabuleiro(u): 
	Reconhecedor - verifica a seguinte lista de parametros
	1) Testa se o argumento tem cumprimento dois.
	2) Testa se o primeiro elemento tem cumprimento dois, se os sub-elementos 
	   desse elemento tem cumprimento identico e se o segundo elemento do
	   argumento e dicionario.
	3) Testa se as especificacoes em t[0] sao inteiros maiores do que zero,
	   e se nao excedem as dimensoes do tabuleiro.
	4) Testa se as chaves de t[1] sao coordenadas e se as suas 
	   chaves sao inteiros no conjunto {0, 1, 2}. '''

'''	pretos_linha(t, l, k): 
	Reconhecedor auxiliar - retorna o numero de celulas com valor 2
	seguidas numa linha (l), de um tabuleiro(t), a partir de uma posicao (k). '''

'''	pretos_coluna(t, c, k): 
	Reconhecedor auxiliar - retorna o numero de celulas com valor 2
	seguidas numa coluna (c), de um tabuleiro(t), a partir de uma posicao (k). '''

'''	sequenciador_linhas(t): 
	Reconhecedor auxiliar - retorna num tuplo, as sequencias de celulas com
	valor 2 seguidas por cada linha do tabuleiro (t).'''

'''	sequenciador_colunas(t): 
	Reconhecedor auxiliar - retorna num tuplo, as sequencias de celulas com
	valor 2 seguidas por cada coluna do tabuleiro (t).'''

'''	tabuleiro_completo(t): 
	Reconhecedor - se o argumento t, for tabuleiro, utiliza os quatro reconhecedores 
	auxiliares do TAD Tabuleiro para verificar se o tabuleiro (t) 
	esta preenchido completa e corretamente (de acordo com as especificacoes). 
	Retorna True ou False. '''

'''	tabuleiros_iguais(t1,t2): 
	Teste - se os argumentos forem tabuleiros, retorna um valor logico
	dependendo da sua igualdade. Testa se as especificacoes, dimensoes e 
	preenchimentos sao iguais. '''

'''	escreve_auxiliar(c): 
	Transformador auxiliar - se c pertencer a {0, 1, 2}, transforma-o em
	'?', '.', ou 'x', respetivamente. '''

'''	escreve_tabuleiro(t): 
	Transformador principal - se o argumento t for um tabuleiro, representa-o.
	Na primeira linha imprime as especificacoes das colunas (por cima das colunas
	respetivas). Na ultima coluna  de cada linha imprime as especificacoes da
	linha respetiva. No interior, imprime os valores de cada celula. '''

# -----------------------------------------------------------------------------#
#                             TAD Tabuleiro CODIGO                             #
# -----------------------------------------------------------------------------#

def cria_tabuleiro(t):  
    if (len(t) ==2 and len(t[0])==len(t[1])):
        somador = 0
        for e in range(0,2): # indica que o ciclo itera o tuplo[0] e o tuplo[1]
        # Verifica se especificacoes sao validas/nao excedem as dimensoes.
            for et in range(len(t[e])):
                if (isinstance(t[e][et], tuple)):
                    for i in range(len(t[e][et])):
                        if (isinstance(t[e][et][i], int) and (t[e][et][i]) > 0):
                            somador += t[e][et][i]
                            if (somador + len(t[e][et]) - 1 > len(t[e])):
                                raise ValueError\
                                      ('cria_tabuleiro: argumentos invalidos')
                        else:
                            raise ValueError\
                                  ('cria_tabuleiro: argumentos invalidos')
                    somador = 0    
    else:
        raise ValueError('cria_tabuleiro: argumentos invalidos')
    tabuleiro = {}
    # Gera chaves do dict, representando coordenadas e coloca os valores a 0.
    for l in range(len(t[0])):
        for c in range(len(t[1])):
            tabuleiro[(l+1,c+1)] = 0
    return (t, tabuleiro)

def tabuleiro_dimensoes(t):
    if not e_tabuleiro(t):
        raise ValueError ('tabuleiro_dimensoes: entrada tem de ser tabuleiro')
    tuplo = t[0]
    return (len(tuplo[0]), len(tuplo[1]))

def tabuleiro_especificacoes(t):
    if not (e_tabuleiro(t)):
        raise ValueError ('tabuleiro_especificacoes: argumentos invalidos')
    return t[0]
            
def tabuleiro_celula(t, xy):
    if not ((e_coordenada(xy)) and \
        coordenada_linha(xy) <= coordenada_linha(tabuleiro_dimensoes(t))\
        and coordenada_coluna(xy) <= coordenada_coluna(tabuleiro_dimensoes(t))\
        and e_tabuleiro(t)):
        raise ValueError('tabuleiro_celula: argumentos invalidos')
    return t[1][cria_coordenada(coordenada_linha(xy), coordenada_coluna(xy))]

def tabuleiro_preenche_celula(t, xy, num):
    if not ((e_tabuleiro(t)) and (e_coordenada(xy)) and \
        coordenada_linha(xy) <= coordenada_linha(tabuleiro_dimensoes(t)) and \
        coordenada_coluna(xy) <= coordenada_coluna(tabuleiro_dimensoes(t)) and \
        (num==0 or num==1 or num==2)):
        raise ValueError ('tabuleiro_preenche_celula: argumentos invalidos')
    else:
        t[1][cria_coordenada(coordenada_linha(xy), coordenada_coluna(xy))] = num
    return t

def e_tabuleiro(u):
	# verifica se as especificacoes tem comprimento 2 (linhas e colunas)
    # verifica se esses 2 tuplos tem a mesma dimensao
    # verifica se as celulas (definidas por coordenadas) estao em dicionarios
    if not (len(u) == 2 and len(u[0]) == 2 and isinstance(u, tuple) \
            and isinstance(u[1], dict)\
            and (len(u[0][0]) - len(u[0][1] == 0))):
        return False

    # verifica se todos os valores das especificacoes sao inteiros maiores que zero
    # e se sao possiveis (nao excedem as dimensoes) 
    somador = 0
    for e in range(0,2):
        for et in range(len(u[0][e])):
            if (isinstance(u[0][e][et], tuple)):
                for i in range(len(u[0][e][et])):
                    if (isinstance(u[0][e][et][i], int) and (u[0][e][et][i])>0):
                        somador += u[0][e][et][i]
                        if (somador + len(u[0][e][et]) - 1 > len(u[0][e])):
                            return False
                    else:
                        return False
                somador = 0

	# verifica se as chaves do dicionario sao coordenadas
    for key in u[1]:
        if not e_coordenada(key):
            return False
    # Verifica se os valores de cada chave-coordenada e legitimo
        for key in u[1]:
            if ((u[1][key]) != 0 and (u[1][key]) != 1 and (u[1][key]) != 2):
                return False
    return True

def pretos_linha(t, l, k):
    maxk = coordenada_linha(tabuleiro_dimensoes(t))
    contador = 0
    # Conta quantas casas pretas existem seguidas numa linha
    for c in range(k, maxk):
        if tabuleiro_celula(t, cria_coordenada(l, c + 1)) == 2:
            for cc in range(c, maxk):
                if tabuleiro_celula(t, cria_coordenada(l, cc + 1)) == 2:
                    contador += 1
                else:
                    return (contador, cc)
            return (contador, cc + 1)
    # Retorna contagem e a coordenada do ultimo preto da sequencia
    return (contador, maxk)


def pretos_coluna(t, c, k):
    maxk = coordenada_linha(tabuleiro_dimensoes(t))
    contador = 0
    # Conta quantas casas pretas existem seguidas numa coluna
    for l in range(k, maxk):
        if tabuleiro_celula(t, cria_coordenada(l + 1, c)) == 2:
            for lc in range(l, maxk):
                if tabuleiro_celula(t, cria_coordenada(lc + 1, c)) == 2:
                    contador += 1
                else:
                    return (contador, lc)
            return (contador, lc + 1)
    # Retorna contagem e a coordenada do ultimo preto da sequencia
    return (contador, maxk)

def sequenciador_linhas(t):
    maxl = coordenada_linha(tabuleiro_dimensoes(t))
    maxk = coordenada_coluna(tabuleiro_dimensoes(t))
    linhas = ()
    # Ciclo que avanca nas linhas
    for l in range(maxl):
        linha = ()
        prox_k = 0
        while prox_k < maxk:
            res = pretos_linha(t, l + 1, prox_k)
            n_pretos = res[0]
            prox_k = res[1]
            if n_pretos != 0:
                linha += (n_pretos,)
        linhas += (linha,)
    # Devolve tuplo de tuplos em que cada tuplo corresponde a uma linha e os
    # os seus subelementos representam o tamanho da sequencia/sequencias de
    # pretos encontrados na respetiva linha
    return linhas

def sequenciador_colunas(t):
    maxl = coordenada_linha(tabuleiro_dimensoes(t))
    maxk = coordenada_coluna(tabuleiro_dimensoes(t))
    colunas = ()
    # Ciclo que avanca nas colunas
    for l in range(maxl):
        coluna = ()
        prox_k = 0
        while prox_k < maxk:
            res = pretos_coluna(t, l + 1, prox_k)
            n_pretos = res[0]
            prox_k = res[1]
            if n_pretos != 0:
                coluna += (n_pretos,)
        colunas += (coluna,)
    # Devolve tuplo de tuplos em que cada tuplo corresponde a uma coluna e os
    # os seus subelementos representam o tamanho da sequencia/sequencias de
    # pretos encontrados na respetiva coluna  
    return colunas

def tabuleiro_completo(t):
    linhas = sequenciador_linhas(t)
    colunas = sequenciador_colunas(t)
    tab = (linhas,) + (colunas,) # Concatena tuplos das funcoes auxiliares
    especs = tabuleiro_especificacoes(t)
    if not (tab == especs): # Compara tuplo concatenado com as especificacoes
        return False
    return True

def tabuleiros_iguais(t1,t2):
    if not (e_tabuleiro(t1) and e_tabuleiro(t2)):
        raise ValueError ('tabuleiros_iguais: argumentos tem de ser tabuleiros')
    dim1 = tabuleiro_dimensoes(t1)
    dim2 = tabuleiro_dimensoes(t2)
    # Verifica se os tabuleiros tem: dimensoes iguais nas linhas e nas colunas
    if not ((coordenada_linha(dim1) - coordenada_linha(dim2) == 0)\
            and (coordenada_coluna(dim1) - coordenada_coluna(dim2) == 0)):
        return False
    # Verifica se as especificacoes do tabuleiro 1 sao iguais as do tabuleiro 2
    if not (tabuleiro_especificacoes(t1) == tabuleiro_especificacoes(t2)):
        return False
    for l in range(coordenada_linha(tabuleiro_dimensoes(t1))):
        for c in range(coordenada_coluna(tabuleiro_dimensoes(t2))):
            # Verifica se as chaves do t1 tem o mesmo valor que as de t2
            if not ((tabuleiro_celula(t1, (l+1,c+1)))\
                    - (tabuleiro_celula(t2, (l+1,c+1))) == 0):
                return False
    return True    

def escreve_tabuleiro(t):
    if not e_tabuleiro(t):
        raise ValueError('escreve_tabuleiro: argumento invalido')
    
    especs = tabuleiro_especificacoes(t)
    linhas = coordenada_linha(tabuleiro_dimensoes(t))
    colunas = coordenada_coluna(tabuleiro_dimensoes(t))

    # calcula a dimensao maxima dos tuplos na especificacoes das colunas
    especs_max = 0
    for i in range(len((especs)[1])):
        if len((especs)[1][i]) > especs_max:
            especs_max = len((especs)[1][i])

    # transforma o tuplo especificacoes das colunas numa manipulavel
    h = [list(x) for x in (tabuleiro_especificacoes(t))[1]]

    # Imprime linha com especificacoes das colunas.
    for v in range(especs_max, 0, -1):
        for i in range(len(h)):
            if len(h[i]) == v:
                print('  %s  ' %(h[i][0]), end='')
                h[i] = h[i][1:]
            else:
                print('     ',end='')
        print('  ')
    
    # Escreve linhas que possuem valores das celulas e especificacoes da linha
    for l in range(linhas):
        for c in range(colunas):
            if c + 1 == colunas:
                # Imprime valor do dicionario entre um espaco e parenteses retos.
                print('[ %s ]' %(escreve_auxiliar(tabuleiro_celula(t,cria_coordenada(l+1,c+1)))), end='')
                for i in range(len((especs)[0][l])):
                    if i + 1 == len((especs)[0][l]):
                        dif = especs_max - (i + 1)
                        espaco = ' '*dif + ' '*dif
                        # Imprime especificacoes linha.
                        print(' %s%s|' %((especs)[0][l][i], espaco), end='')
                    else:
                        print(' %s' %((especs)[0][l][i]), end='')
            else:
                print('[ %s ]' %(escreve_auxiliar(tabuleiro_celula(t,cria_coordenada(l+1,c+1)))), end='')
        print ('')
    print('')
    return

# Faz corresponder a cada valor de uma chave-coordenada o simbolo respetivo.
def escreve_auxiliar(c):
    dic = {'0':'?', '1':'.', '2':'x'}
    return (dic[str(c)])

# -----------------------------------------------------------------------------#
#                                TAD Jogada HELP                               #
# -----------------------------------------------------------------------------#

'''	cria_jogada(xy, v): 
	Construtor - se os argumentos forem uma coordenada (xy) e um valor (v)
	pertencente a {0, 1, 2}, retorna uma jogada'''

'''	jogada_coordenada(j): 
	Seletor - se o argumento for uma jogada (j), retorna a sua coordenada,
	sob a forma de tuplo. '''

'''	jogada_valor(j): 
	Seletor - se o argumento for uma jogada (j), retorna o seu valor,
	sob a forma de inteiro. '''

'''	e_jogada(u): 
	Reconhecedor - retorna True se o argumento (u) for uma jogada e False se 
	nao for. Testa se e um tuplo composto por um tuplo de coordenadas na 
	primeira posicao e um inteiro pertencente a {0, 1, 2} na segunda. '''

'''	jogadas_iguais(j1, j2): 
	Teste - se os argumentos forem jogadas (j1, j2) retorna um valor logico dependendo
	da sua igualdade. Compara as coordenadas e os valores. '''

'''	jogada_para_cadeia(j): 
	Transformador - se o argumento for jogada (j), representa-a na forma
	'(x : y) --> v'. '''

# -----------------------------------------------------------------------------#
#                               TAD Jogada CODIGO                              #
# -----------------------------------------------------------------------------#

def cria_jogada(xy, v):
    if not(e_coordenada(xy) and (v==1 or v==2)):
        raise ValueError ('cria_jogada: argumentos invalidos')
    return (xy, v)

def jogada_coordenada(j):
    if not e_jogada(j):
        raise ValueError ('jogada_coordenada: entrada tem de ser uma jogada')
    return j[0]

def jogada_valor(j):
    if not e_jogada(j):
            raise ValueError ('jogada_coordenada: entrada tem de ser uma jogada')
    return j[1]

def e_jogada(u):
    if isinstance(u, (tuple)):
        if len(u)==2:
            return (e_coordenada(u[0]) and (u[1]==1 or u[1]==2))
    return False

def jogadas_iguais(j1, j2):
    if not (e_jogada(j1) and e_jogada(j2)):
        raise ValueError ('jogadas_iguais: entradas tem de ser jogadas')
    if not (coordenadas_iguais(jogada_coordenada(j1), jogada_coordenada(j2))\
    	and jogada_valor(j1)-jogada_valor(j2)==0):
        return False     
    return True

def jogada_para_cadeia(j):
    if not e_jogada(j):
        raise ValueError ('jogada_para_cadeia: entrada tem de ser jogada')
    
    # Representacao a jogada (coordenada e valor)
    return ((coordenada_para_cadeia(jogada_coordenada(j)))\
            + ' --> ' + str(jogada_valor(j)))


# -----------------------------------------------------------------------------#
#                           Funcoes Adicionais HELP                            #
# -----------------------------------------------------------------------------#

'''	le_tabuleiro(txt):
	le a primeira linha do ficheiro e retorna as especificacoes para um
	tabuleiro. '''

'''	pede_jogada(t): 
	Se o argumento for um tabuleiro pede uma coordenada e um valor.
	Retorna uma jogada composta pela coordenada e o valor introduzidos. '''
	
'''	tabuleiro_celeluas_vazias(t): 
	Teste auxiliar - se o argumento for tabuleiro, retorna uma lista das
	coordenadas cujas celulas tem valor 0. '''

'''	jogo_picross(txt): 
	Funcao que engloba todas as funcoes necessarias para o funcionamento do jogo. 
	Comeca por ler o ficheiro com especificacoes do tabuleiro, de seguida gera 
	um tabuleiro, escreve a sua representacao externa e vai pedindo jogadas
	ate o tabuleiro estar preenchido (corretamente ou nao). 
	Quando o tabuleiro estiver totalmente preenchido retorna True ou False
	dependendo de estar preenchido de acordo com as especificacoes. '''

# -----------------------------------------------------------------------------#
#                          Funcoes Adicionais CODIGO                           #
# -----------------------------------------------------------------------------#

def le_tabuleiro(txt):
    # Le a linha do ficheiro e retorna as especificacoes para um tabuleiro
    return eval((open(txt, 'r')).readline())


def pede_jogada(t):
    if not e_tabuleiro(t):
	    raise ValueError('pede_jogada: argumento invalido')

    maxdim = tabuleiro_dimensoes(t)
    # Variavel que gera mensagem para o input consoante pedido no enunciado
    msg = 'Introduza a jogada\n- coordenada entre (1 : 1) e ' + \
        coordenada_para_cadeia(maxdim) +' >> '

    # Transforma a coordenada_para_cadeia do input numa coordenada
    xy = eval((input(msg)).replace(' : ' , ', '))
    valor = int(input('- valor >> '))
    
    
    # Verifica se a coordenada e valida para o tabuleiro do jogo
    if (coordenada_linha(xy) and coordenada_coluna(xy)):
        if (coordenada_linha(xy) <= coordenada_linha(maxdim) and \
	    coordenada_coluna(xy) <= coordenada_coluna(maxdim)):
            return cria_jogada(cria_coordenada(coordenada_linha(xy), coordenada_coluna(xy)), valor)
    return False

def tabuleiro_celulas_vazias(t):
    if not e_tabuleiro(t):
	    raise ValueError('tabuleiro_celulas_vazias: argumento invalido')

    maxl = coordenada_linha(tabuleiro_dimensoes(t))
    maxc = coordenada_coluna(tabuleiro_dimensoes(t))
    celulas_vazias = []
    # Verifica coordenadas do tabuleiro sem valor atribuido e caso encontre co-
    # loca a sua coordenada na lista
    for l in range(maxl):
        for c in range(maxc):
            if tabuleiro_celula(t, cria_coordenada(l+1,c+1)) == 0:
                celulas_vazias += [cria_coordenada(l+1,c+1)]
    return celulas_vazias

def jogo_picross(txt):
    print('JOGO PICROSS')
    E = le_tabuleiro(txt)
    T = cria_tabuleiro(E)
    I = escreve_tabuleiro(T)
    # Enquanto existirem celulas por preencher, continua a pedir jogadas
    while len(tabuleiro_celulas_vazias(T)) != 0:
        J = pede_jogada(T)
        if J == False:
            print('Jogada invalida')
        else:
            escreve_tabuleiro(tabuleiro_preenche_celula(\
                T, jogada_coordenada(J), jogada_valor(J)))
    if tabuleiro_completo(T):
        print('JOGO: Parabens, encontrou a solucao!')
        return True
    return False

# -----------------------------------------------------------------------------#
#                                FIM DE CODIGO                                 #
# -----------------------------------------------------------------------------#