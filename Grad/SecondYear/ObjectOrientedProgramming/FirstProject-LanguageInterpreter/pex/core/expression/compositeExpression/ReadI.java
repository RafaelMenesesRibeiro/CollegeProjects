package pex.core.expression.compositeExpression;

import pex.core.visitor.Visitor;
import pex.core.visitor.Visitable;
import pex.core.Program;
import pex.core.expression.CompositeExpression;
import pex.core.expression.Literal;
import pex.core.expression.literal.IntegerLiteral;
import pex.AppIO;

public class ReadI extends CompositeExpression implements Visitable{
	private AppIO _appIO;

	public ReadI(Program prog){
		_appIO = prog.getInterpreter().getAppIO();
	}
	
	public Literal evaluate(){
		return new IntegerLiteral(_appIO.readInteger(""));
	}

	public String getAsText(){
		return "(readi)";
	}
	
	public void accept(Visitor v){
		v.visit(this);
	}
}