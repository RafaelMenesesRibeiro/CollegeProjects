## Relatório de análise dos testes JMeter ##

# Introdução #
- Os testes 100Writes e 30Writes têm como objetivo processar 100 Aventuras (adventures) 5 vezes cada uma, para que chegassem ao estado Confirmed. No entanto, no teste 30Writes, o processamento não foi completamente implementado, sendo as aventuras processadas apenas 4 vezes.
- O teste de 100Reads também cria 100 Aventuras (adventures). No entanto, não as processa. Apenas lê os vários dados

# 100 Reads #
## Sequência do teste ##
Após a inicialização de todos os módulos, é lido tudo o que foi criado, pela seguinte ordem:
1. Read Brokers
2. Read Adventures
3. Read Banks
4. Read Clients
5. Read Accounts
6. Read Hotels
7. Read Rooms
8. Read Providers
9. Read Activities
10. Read Offers

# 100 Writes #
## Sequência do teste ##
Após a inicialização de todos os módulos, todas as 100 aventuras (Adventures) são processadas até ao estado Confirmed (5 vezes).

# 30 Writes #
## Sequência do teste ##
Após a inicialização de todos os módulos, é lido tudo o que foi criado, pela seguinte ordem:
1. Read Brokers
2. Read Adventures
3. Read Banks
4. Read Clients
5. Read Accounts
6. Read Hotels
7. Read Rooms
8. Read Providers
9. Read Activities
10. Read Offers
11. Process Adventures

# Erros #
Em relação aos erros durante o teste, foram encontrados erros com 200 utilizadores concorrentes, embora isso dependa da máquina onde o teste está a ser executado.

# Conclusão #
Quanto maior o número de utilizadores concorrentes, maior será o número de conflitos de concorrência. Tal implica que o tempo de acesso irá aumentar com o número de utilziadores concorrentes.


