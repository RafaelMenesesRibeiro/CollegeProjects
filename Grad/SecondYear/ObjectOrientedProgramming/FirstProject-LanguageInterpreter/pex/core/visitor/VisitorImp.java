package pex.core.visitor;

import java.util.*;

import pex.core.visitor.Visitor;
import pex.core.IdentifierComparator;
import pex.core.Expression;
import pex.core.expression.Literal;
import pex.core.expression.Identifier;
import pex.core.expression.compositeExpression.*;
import pex.core.expression.compositeExpression.binaryExpression.Set;

public abstract class VisitorImp implements Visitor{
	protected TreeSet<Identifier> _setIdentifier;

	public VisitorImp(){
		_setIdentifier = new TreeSet<Identifier>(new IdentifierComparator());
	}

	public void visit(Literal object){
		return;
	}

	public abstract void visit(Identifier object);
	
	public void visit(ReadI object){
		return;
	}
	
	public void visit(ReadS object){
		return;
	}
	
	public abstract void visit(Set object);

	public void visit(UnaryExpression object){
		object.getArgument().accept(this);
	}
	
	public void visit(BinaryExpression object){
		object.getFirstArgument().accept(this);
		object.getSecondArgument().accept(this);
	}
	
	public void visit(TrenaryExpression object){		
		object.getFirstArgument().accept(this);
		object.getSecondArgument().accept(this);
		object.getThirdArgument().accept(this);
	}
	
	public void visit(VariadicExpression object){
		ArrayList<Expression> exps = (ArrayList<Expression>)object.getArguments();
		for (Expression exp : exps)
			exp.accept(this);
	}

	public TreeSet<Identifier> getIdentifierSet(){
		return _setIdentifier;
	}
}