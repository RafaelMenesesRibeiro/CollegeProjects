package pex.core.expression.compositeExpression.variadicExpression;

import pex.core.Program;
import pex.core.Expression;
import pex.core.expression.compositeExpression.VariadicExpression;
import pex.core.expression.Literal;
import pex.core.expression.literal.StringLiteral;
import pex.AppIO;
import java.util.*;

public class Print extends VariadicExpression{
	private AppIO _appIO;

	public Print(List<Expression> exps, Program prog){
		super(exps);
		_appIO = prog.getInterpreter().getAppIO();
	}

	public Literal evaluate(){
		List<Expression> exps = super.getArguments();
		Literal last = null;
		String res = "";    
		for (Expression aux : exps){
			last = aux.evaluate();
			if (last instanceof StringLiteral){
				StringLiteral l1 = (StringLiteral) last;
				res += l1.stringValue();
			}
			else
				res += last.getAsText();
		}
		_appIO.println(res);
		return last;
	}

	protected String getOperationName(){
		return "print";
	}
}