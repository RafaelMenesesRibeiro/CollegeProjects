package pex.core.expression.compositeExpression;

import pex.core.visitor.Visitor;
import pex.core.visitor.Visitable;
import pex.core.Program;
import pex.core.expression.CompositeExpression;
import pex.core.expression.Literal;
import pex.core.expression.literal.StringLiteral;
import pex.AppIO;

public class ReadS extends CompositeExpression implements Visitable{
	private AppIO _appIO;

	public ReadS(Program prog){
		_appIO = prog.getInterpreter().getAppIO();
	}

	public Literal evaluate(){
		return new StringLiteral(_appIO.readString(""));
	}

	public String getAsText(){
		return "(reads)";
	}
	
	public void accept(Visitor v){
		v.visit(this);
	}
}