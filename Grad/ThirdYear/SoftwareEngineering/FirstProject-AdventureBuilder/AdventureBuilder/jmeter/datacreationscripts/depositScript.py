
for i in range (1, 203):
	bankCode = "BK12"
	ID = i
	iban = "BK12{}".format(i + 202)
	amount = "100000"
	line = "{},{},{},{}".format(bankCode, ID, iban, amount)
	print(line)