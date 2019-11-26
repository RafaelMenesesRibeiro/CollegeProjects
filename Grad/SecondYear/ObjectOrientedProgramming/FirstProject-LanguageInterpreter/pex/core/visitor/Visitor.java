package pex.core.visitor;

import pex.core.expression.Literal;
import pex.core.expression.Identifier;
import pex.core.expression.compositeExpression.*;
import pex.core.expression.compositeExpression.binaryExpression.Set;

public interface Visitor{
    public void visit(Literal object);
    public void visit(Identifier object);
    public void visit(ReadI object);
    public void visit(ReadS object);
    public void visit(Set object);
    public void visit(UnaryExpression object);
    public void visit(BinaryExpression object);
    public void visit(TrenaryExpression object);
    public void visit(VariadicExpression object);
}