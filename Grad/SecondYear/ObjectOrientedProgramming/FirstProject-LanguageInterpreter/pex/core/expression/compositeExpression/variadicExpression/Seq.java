package pex.core.expression.compositeExpression.variadicExpression;

import pex.core.Expression;
import pex.core.expression.compositeExpression.VariadicExpression;
import pex.core.expression.Literal;
import java.util.*;

public class Seq extends VariadicExpression{
	public Seq(List<Expression> exps){
		super(exps);
	}

    public Literal evaluate(){
        List<Expression> exps = (List<Expression>)super.getArguments();
        Literal returnValue = null;
        for (Expression aux : exps)
            returnValue = aux.evaluate();
        return returnValue;
    }

	protected String getOperationName(){
		return "seq";
	}
}