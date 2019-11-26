package pex.core.expression.compositeExpression.binaryExpression;

import pex.core.Expression;
import pex.core.expression.compositeExpression.BinaryExpression;
import pex.core.expression.Literal;
import pex.core.expression.literal.IntegerLiteral;

public class While extends BinaryExpression{
	public While(Expression exp1, Expression exp2){
		super(exp1, exp2);
	}

	public Literal evaluate(){
		Expression e1 = super.getFirstArgument();
		Expression e2 = super.getSecondArgument();
		Literal l1 = null;
		while (true){ 
			l1 = e1.evaluate();
			if (this.getIntValue(l1) != 0)
				e2.evaluate();
			else
				return l1;
		}
	}

	protected String getOperationName(){
		return "while";
	}
}