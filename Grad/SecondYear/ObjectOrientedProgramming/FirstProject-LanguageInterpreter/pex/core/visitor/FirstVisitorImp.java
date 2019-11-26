package pex.core.visitor;

import java.util.*;

import pex.core.visitor.VisitorImp;
import pex.core.expression.Identifier;
import pex.core.expression.compositeExpression.binaryExpression.Set;

public class FirstVisitorImp extends VisitorImp{
	public FirstVisitorImp(){
		super();
	}
	
	@Override
	public void visit(Identifier object){ 
		_setIdentifier.add(object);
	}
	
	@Override
	public void visit(Set object){
		object.getFirstArgument().accept(this);
		object.getSecondArgument().accept(this);
	}
}