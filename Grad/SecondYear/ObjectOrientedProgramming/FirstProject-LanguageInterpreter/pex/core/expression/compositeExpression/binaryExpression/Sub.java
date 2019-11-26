package pex.core.expression.compositeExpression.binaryExpression;

import pex.core.Expression;
import pex.core.expression.compositeExpression.BinaryExpression;
import pex.core.expression.Literal;
import pex.core.expression.literal.IntegerLiteral;

public class Sub extends BinaryExpression{
	public Sub(Expression exp1, Expression exp2){
		super(exp1, exp2);
	}

	public Literal evaluate(){
		int a = this.getIntValue(super.getFirstArgument().evaluate());
		int b = this.getIntValue(super.getSecondArgument().evaluate());
		return new IntegerLiteral(a - b);
	}

	protected String getOperationName(){
		return "sub";
	}
}