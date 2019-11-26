package pex.core.expression.compositeExpression.unaryExpression;

import pex.core.Expression;
import pex.core.expression.compositeExpression.UnaryExpression;
import pex.core.expression.Literal;
import pex.core.expression.literal.IntegerLiteral;

public class Neg extends UnaryExpression{
	public Neg(Expression exp1){
		super(exp1);
	}
	
	public Literal evaluate(){
		int a = this.getIntValue(super.getArgument().evaluate());
		return new IntegerLiteral(-a);
	}

	protected String getOperationName(){
		return "neg";
	}
}