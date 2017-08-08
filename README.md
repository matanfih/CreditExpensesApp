# CreditExpensesApp
basic app to handle credit card balance 
basic steps 
	* read config
		templete
		dictionary
	* read execl 
		if template found 
			start parsing
		else
			prompt user to set templte (name index , sum index , start line)
	* parse execl
		foreach line
			try to find bussiness in dictionary
			if (not found) add it to mising value)
			else calc total and keep track 
	* print result 
