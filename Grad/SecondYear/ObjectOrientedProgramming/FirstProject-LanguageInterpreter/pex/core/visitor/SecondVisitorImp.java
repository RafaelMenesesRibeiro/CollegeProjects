package pex.core.visitor;

import java.util.*;

import pex.core.visitor.VisitorImp;
import pex.core.expression.Identifier;
import pex.core.expression.compositeExpression.binaryExpression.Set;

public class SecondVisitorImp extends VisitorImp{
	public SecondVisitorImp(){
		super();
	}
	
	@Override
	public void visit(Identifier identifierObject){
		return;
	}
	
	@Override
	public void visit(Set object){
		_setIdentifier.add((Identifier)object.getFirstArgument());
		object.getSecondArgument().accept(this);
	}
}