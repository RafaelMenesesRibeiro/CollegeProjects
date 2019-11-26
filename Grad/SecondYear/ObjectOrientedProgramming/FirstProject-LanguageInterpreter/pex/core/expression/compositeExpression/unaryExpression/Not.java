package pex.core.expression.compositeExpression.unaryExpression;

import pex.core.Expression;
import pex.core.expression.compositeExpression.UnaryExpression;
import pex.core.expression.Literal;
import pex.core.expression.literal.IntegerLiteral;

public class Not extends UnaryExpression{
	public Not(Expression exp1){
		super(exp1);
	}
	
	public Literal evaluate(){
		IntegerLiteral l1 = (IntegerLiteral)super.getArgument().evaluate();
		if (l1.intValue() == 0)
			return new IntegerLiteral(1);
		return new IntegerLiteral(0);		
	}

	protected String getOperationName(){
		return "not";
	}
}