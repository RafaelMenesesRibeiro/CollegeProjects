package pex.core;

import java.lang.ClassCastException;

import pex.core.visitor.Visitor;
import pex.core.visitor.Visitable;
import pex.core.expression.Literal;
import pex.core.expression.literal.IntegerLiteral;

public abstract class Expression implements java.io.Serializable{
	public abstract Literal evaluate();
	public abstract String getAsText();
	public abstract void accept(Visitor v);
	
	public int getIntValue(Literal lit){
		try{
			return ((IntegerLiteral)lit).intValue();
		}
		catch(ClassCastException e){
			e.printStackTrace();
			return 0;
		}
	}
}