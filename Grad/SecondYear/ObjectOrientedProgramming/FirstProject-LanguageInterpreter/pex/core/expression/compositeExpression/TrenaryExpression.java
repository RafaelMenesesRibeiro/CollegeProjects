package pex.core.expression.compositeExpression;

import pex.core.visitor.Visitor;
import pex.core.visitor.Visitable;
import pex.core.expression.CompositeExpression;
import pex.core.Expression;

public abstract class TrenaryExpression extends CompositeExpression implements Visitable{
	private Expression _expression1;
	private Expression _expression2;
	private Expression _expression3;

	public TrenaryExpression(Expression exp1, Expression exp2, Expression exp3){
		_expression1 = exp1;
		_expression2 = exp2;
		_expression3 = exp3;
	}

	public Expression getFirstArgument(){
		return _expression1;
	}

	public Expression getSecondArgument(){
		return _expression2;
	}
	
	public Expression getThirdArgument(){
		return _expression3;
	}

	public String getAsText(){
		return "(" + getOperationName() + " " + getFirstArgument().getAsText() + " " + getSecondArgument().getAsText() + " " + getThirdArgument().getAsText() + ")";
	}
	
	protected abstract String getOperationName();
	
	public void accept(Visitor v){
		v.visit(this);
	}
}