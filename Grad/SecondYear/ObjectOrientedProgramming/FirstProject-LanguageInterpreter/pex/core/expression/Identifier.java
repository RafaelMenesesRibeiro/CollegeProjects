package pex.core.expression;

import java.util.*;

import pex.core.visitor.Visitor;
import pex.core.visitor.Visitable;
import pex.core.Program;
import pex.core.Expression;
import pex.core.expression.literal.IntegerLiteral;

public class Identifier extends Expression{
	private Program _program;
	private String _name;

	public Identifier(Program prog, String name){
		if (!Character.isLetter(name.charAt(0)))
			return;
		_program = prog;
		_name = name;
		_program.identifierAdd(this);
	}

	public String getName(){
		return _name;
	}

	public String getProgramName(){
		return _program.getName();
	}

	public String getAsText(){
		return _name;
	}

	public Literal evaluate(){
		Literal value = _program.getIdentifierValue(this);
		if (value != null)
			return value;
		return new IntegerLiteral(0);
	}

	public void setValue(Literal value){
		_program.setIdentifierValue(this, value);
	}
	
	public void accept(Visitor v){
		v.visit(this);
	}
}