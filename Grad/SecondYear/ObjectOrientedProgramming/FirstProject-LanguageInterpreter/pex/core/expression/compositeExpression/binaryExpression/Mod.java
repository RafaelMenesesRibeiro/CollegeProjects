package pex.core.expression.compositeExpression.binaryExpression;

import java.lang.ArithmeticException;

import pex.core.Expression;
import pex.core.expression.compositeExpression.BinaryExpression;
import pex.core.expression.Literal;
import pex.core.expression.literal.IntegerLiteral;

public class Mod extends BinaryExpression{
	public Mod(Expression exp1, Expression exp2){
		super(exp1, exp2);
	}

	public Literal evaluate(){
		try{
			int a = this.getIntValue(super.getFirstArgument().evaluate());
			int b = this.getIntValue(super.getSecondArgument().evaluate());
			return new IntegerLiteral(a % b);
		}
		catch(ArithmeticException e){
			e.printStackTrace();
			return new IntegerLiteral(0);
		}
	}

	protected String getOperationName(){
		return "mod";
	}
}