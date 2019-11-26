package pex.core.expression;

import pex.core.visitor.Visitor;
import pex.core.visitor.Visitable;
import pex.core.Expression;

public abstract class Literal extends Expression{
	public abstract Literal evaluate();
	public abstract String getAsText();
	
	public void accept(Visitor v){
		v.visit(this);
	}
}