package pex.core.expression.literal;

import pex.core.expression.Literal;

public class StringLiteral extends Literal{
	private String _value;

	public StringLiteral(String value){
		_value = value;
	}

	public Literal evaluate(){
		return this;
	}

	public String getAsText(){
		return "\"" + _value + "\"";
	}

	public String stringValue(){
		return _value;
	}
}