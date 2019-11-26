package pex.core.expression.compositeExpression;

import pex.core.visitor.Visitor;
import pex.core.visitor.Visitable;
import pex.core.expression.CompositeExpression;
import pex.core.Expression;

public abstract class BinaryExpression extends CompositeExpression implements Visitable{
	private Expression _expression1;
	private Expression _expression2;

	public BinaryExpression(Expression exp1, Expression exp2){
		_expression1 = exp1;
		_expression2 = exp2;
	}

	public Expression getFirstArgument(){
		return _expression1;
	}

	public Expression getSecondArgument(){
		return _expression2;
	}

	public String getAsText(){
		return "(" + getOperationName() + " " + getFirstArgument().getAsText() + " " + getSecondArgument().getAsText() + ")";
	}
	
	protected abstract String getOperationName();
	
	public void accept(Visitor v){
		v.visit(this);
	}
}