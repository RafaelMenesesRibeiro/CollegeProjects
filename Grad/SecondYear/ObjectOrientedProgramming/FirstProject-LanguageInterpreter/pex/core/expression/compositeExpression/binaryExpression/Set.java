package pex.core.expression.compositeExpression.binaryExpression;

import pex.core.visitor.Visitor;
import pex.core.visitor.Visitable;
import pex.core.Program;
import pex.core.Expression;
import pex.core.expression.Identifier;
import pex.core.expression.compositeExpression.BinaryExpression;
import pex.core.expression.Literal;

public class Set extends BinaryExpression implements Visitable{
	private Program _prog;

	public Set(Program prog, Expression exp1, Expression exp2){
		super(exp1, exp2);
		_prog = prog;
		_prog.setIdentifierValue((Identifier)super.getFirstArgument(), null);
	}

	public Literal evaluate(){
		Literal value = super.getSecondArgument().evaluate();
		_prog.setIdentifierValue((Identifier)super.getFirstArgument(), value);
		return value;
	}

	protected String getOperationName(){
		return "set";
	}
	
	@Override
	public void accept(Visitor v){
		v.visit(this);
	}
}