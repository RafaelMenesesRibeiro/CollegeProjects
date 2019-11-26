package pex.core.expression.compositeExpression;

import pex.core.visitor.Visitor;
import pex.core.visitor.Visitable;
import pex.core.expression.CompositeExpression;
import pex.core.Expression;
import java.util.*;

public abstract class VariadicExpression extends CompositeExpression implements Visitable{
	private List<Expression> _exps;

	public VariadicExpression(List<Expression> exps){
		_exps = new ArrayList<Expression>();
		_exps.addAll(0, exps);
	}

	public List<Expression> getArguments(){
		return _exps;
	}

	public Expression getLast(){
		return _exps.get(_exps.size() - 1);
	}

	public String getAsText(){
		String res = "(" + getOperationName();
		for (Expression aux : _exps)
			res += " " + aux.getAsText();
		res += ")";
		return res;
	}

	protected abstract String getOperationName();
	
	public void accept(Visitor v){
		v.visit(this);
	}
}