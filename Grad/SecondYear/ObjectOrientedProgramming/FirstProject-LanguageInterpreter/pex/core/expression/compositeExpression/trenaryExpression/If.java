package pex.core.expression.compositeExpression.trenaryExpression;

import pex.core.Expression;
import pex.core.expression.compositeExpression.TrenaryExpression;
import pex.core.expression.Literal;

public class If extends TrenaryExpression{
	public If(Expression exp1, Expression exp2, Expression exp3){
		super(exp1, exp2, exp3);
	}

	public Literal evaluate(){
		int a = this.getIntValue(super.getFirstArgument().evaluate());
		if (a != 0)
			return super.getSecondArgument().evaluate();
		return super.getThirdArgument().evaluate();
	}

	protected String getOperationName(){
		return "if";
	}
}