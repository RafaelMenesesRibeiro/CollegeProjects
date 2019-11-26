package pex.core.expression.literal;

import pex.core.expression.Literal;

public class IntegerLiteral extends Literal{
	private int _value;

	public IntegerLiteral(int value){
		_value = value;
	}

	public Literal evaluate(){
		return this;
	}

	public String getAsText(){
		return "" + _value;
	}

	public int intValue(){
		return _value;
	}
}