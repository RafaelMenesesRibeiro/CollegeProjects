package pex.core.expression.compositeExpression;

import pex.core.visitor.Visitor;
import pex.core.visitor.Visitable;
import pex.core.expression.CompositeExpression;
import pex.core.Expression;

public abstract class UnaryExpression extends CompositeExpression implements Visitable{
	private Expression _expression1;

	public UnaryExpression(Expression exp1){
		_expression1 = exp1;
	}

	public Expression getArgument(){
		return _expression1;
	}

	public String getAsText(){
		return "(" + getOperationName() + " " + getArgument().getAsText() + ")";
	}
	
	protected abstract String getOperationName();
	
	public void accept(Visitor v){
		v.visit(this);
	}
}