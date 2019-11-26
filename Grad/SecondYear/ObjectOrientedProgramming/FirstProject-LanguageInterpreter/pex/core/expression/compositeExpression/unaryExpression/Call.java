package pex.core.expression.compositeExpression.unaryExpression;

import pex.core.Program;
import pex.core.Expression;
import pex.core.expression.compositeExpression.UnaryExpression;
import pex.core.expression.Literal;
import pex.core.expression.literal.StringLiteral;

public class Call extends UnaryExpression{
	private Program _prog;

	public Call(Program prog, Expression exp1){
		super(exp1);
		_prog = prog;
	}

	public Literal evaluate(){
		StringLiteral l1 = (StringLiteral)super.getArgument();
		return _prog.getInterpreter().programExecute(l1.stringValue());
	}
	
	protected String getOperationName(){
		return "call";
	}
}