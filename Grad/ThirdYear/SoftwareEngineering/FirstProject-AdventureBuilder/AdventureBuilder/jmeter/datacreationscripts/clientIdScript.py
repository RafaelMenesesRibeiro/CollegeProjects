
for i in range (1, 203):
	bankCode = "BK12"
	ID = i
	line = "{},{}".format(bankCode, ID)
	clientNif = "21111100{}".format(i)
	print(line)