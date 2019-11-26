package pex.core.visitor;

import pex.core.visitor.Visitor;

public interface Visitable{
	public void accept(Visitor v);
}