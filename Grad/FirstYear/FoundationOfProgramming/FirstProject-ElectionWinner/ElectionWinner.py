# Francisco Teixeira de Barros 85069, Rafael M. Lucas Ribeiro 84758, TP tg079

# ------------------------ PRESSUPOSTOS DO EXERCICIO ------------------------- #
# As candidaturas com as siglas\nomes de cada partido ja estao predefinidos,
# Assim como a lista mpcirculo com mandatos a distribuir por cada circulo e
# Ainda o tuplo votacoes.

candidaturas = ['PDR\tPartido Democratico Republicano',
                'PCP-PEV\tCDU - Coligacao Democratica Unitaria',
                'PPD/PSD-CSD/PP\tPortugal a Frente',
                'MPT\tPartido da Terra','L/TDA\tLIVRE/Tempo de Avancar',
                'PAN\tPessoas-Animais-Natureza',
                'PTP-MAS\tAgir',
                'JPP\tJuntos pelo Povo',
                'PNR\tPartido Nacional Renovador',
                'PPM\tPartido Popular Monarquico',
                'NC\tNos, Cidadaos!',
                'PCTP/MRPP\tPartido Comunista dos Trabalhadores Portugueses',
                'PS\tPartido Socialista',
                'B.E.\tBloco de Esquerda',
                'PURP\tPartido Unidos dos Reformados e Pensionistas']

mpcirculo = [16,3,19,3,4,9,3,9,4,10,47,2,39,9,18,6,5,9,5,6,2,2]

'''Funcao mandatos: Distribui mandatos de um circulo eleitoral, utilizando o
metodo DHondt. Em que nr_mandatos e o inteiro que representa o numero de
mandatos a distribuir e o tuplo nr_votos indica quantas candidaturas existem e 
quantos votos cada uma recebeu.'''

def mandatos (nr_mandatos, nr_votos):
    votos_tmp = list(nr_votos)[:] 
    mandatos_c = [0] * len(nr_votos)  # Acumula mandatos por candidatura.
    divisor = [1] * len(nr_votos)  # Valores dos divisores de cada candidatura.
    lista_empate = []  # Utilizada para determinar o vencedor em caso de empate. 
    mandatosatribuidos = 0

    # maisv e a variavel que detecta a candidatura mais votada em votos_tmp.
    # mais_v percorre votos_tmp em busca de candidaturas cujo o valor na
    # votos_temp e igual ao maximo.
    # Se existirem varias candidaturas com valor igual a maisv considera-se 
    # empate e a candidatura com o menos votos totais (considerada no empate
    # recebe o mandato.
    # O ciclo repete-se ate todos os mandatos estarem distribuidos.
    
    while mandatosatribuidos < nr_mandatos:
        maisv = max(votos_tmp)    
        mais_v = [v for v in range(len(votos_tmp)) if votos_tmp[v] == maisv]
        
        if len(mais_v) > 1:
            for m in mais_v:
                lista_empate = lista_empate + [nr_votos[m]]
                proximac = mais_v[lista_empate.index(min(lista_empate))]
            lista_empate = []
        
        else:
            proximac = votos_tmp.index(max(votos_tmp))
       
        pc = proximac
        mandatos_c[pc] += 1
        divisor[pc] += 1
        votos_tmp[pc] = (nr_votos[pc] / divisor[pc])       
        mandatosatribuidos += 1
        
    return tuple(mandatos_c)

'''Funcao assembleia: Utiliza a funcao mandatos para calcular os mandatos a dis-
tribuir por cada um dos circulos eleitorais e devolve a acumulacao de mandatos
atribuidos por partido em cada um desses circulos. O resultado corresponde ao 
numero de lugares na assembleia da republica atribuidos a cada candidatura.'''

def assembleia (votacoes):
    
    acumulacao_c = [0] * 15  # Lista na qual se acumulam os mandatos atribuidos.
    
    # O primeiro ciclo calcula os mandatos atribuidos em cada circulo.
    # O segundo ciclo atribui a acumulacao temporaria a acumulacao inicial.
    
    for v in range(len(votacoes)):
        acumulacao_temp = mandatos(mpcirculo[v], votacoes[v])
        
        for a in range(len(acumulacao_temp)):
            acumulacao_c[a] = acumulacao_temp[a] + acumulacao_c[a]
    return tuple(acumulacao_c)

'''Funcao max_mandatos: utiliza a funcao assembleia para apresentar a
candidatura com o maior numero de mandatos atribuidos.'''

def max_mandatos (votacoes):
    # maiorc e a candidatura com maior acumulacao de mandatos em acumulacao_c.
    # maior_c e a lista gerada pela verificacao de acumulacao_c, a qual se usa
    # para verificar se existem empates de mandatos acumulados entre partidos.
    
    acumulacao_c = list(assembleia(votacoes))
    maiorc = max(acumulacao_c)
    maior_c = [a for a in range(len(acumulacao_c)) if acumulacao_c[a] == maiorc]
    
    if len(maior_c) > 1:
        maiorc = "Empate tecnico"
    else:
        maiorc = candidaturas[acumulacao_c.index(max(acumulacao_c))]    
       
    return maiorc