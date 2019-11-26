/*
* PO, 2nd Project
* 2016/2017 - 1st Semestre
* Instituto Superior Tecnico
* U.C. Programacao com objetos
* Diogo Vilela, nº 84710
* Rafael Ribeiro, nº 84758
*/
package pex.core;

import java.util.*;
import java.lang.*;
import pex.parser.ParserException;

import pex.parser.NewParser;
import pex.core.visitor.FirstVisitorImp;
import pex.core.visitor.SecondVisitorImp;
import pex.core.Expression;
import pex.core.expression.Literal;
import pex.core.expression.Identifier;
import pex.core.expression.literal.StringLiteral;
import pex.core.expression.literal.IntegerLiteral;

/**
* This class (Program) represents a set of Expression instances that can be
* executed to result in a single Literal instance.
* 
* @author Diogo Vilela
* @author Rafael Ribeiro
* @version 1.0
*/
public class Program implements java.io.Serializable{
	/**
	* Holds the Interpreter instance that has this Program instance.
	*/
	private Interpreter _interp;

	/**
	* Holds the name of the Program instance.
	*/
	private String _name;

	/**
	* Holds the collection of Expression instances that the program contains.
	*/
	private List<Expression> _exps;

	/**
	* Constructor.
	* Creates an instance of the class Program. Initializes the _name fields
	* with the given String parameter and creates
	*
	* @param	name	The name of the program that is being created
	*/
	public Program(Interpreter interp, String name){
		_interp = interp;
		_name = name;
		_exps = new ArrayList<Expression>();
	}

	/**
	* Executes the program and returns the Literal instance that contains the
	* result.
	*
	* @return	Literal instance that represents the result
	*/
	public Literal execute(){
		Literal l1 = new IntegerLiteral(0);
		for (Expression aux : _exps)
			l1 = aux.evaluate();
		return l1;
	}

	/**
	* Returs the Array of Expression instances. Used to list the Identifiers
	* that weren't initialized with the Set operator and all of them, separately,
	* that are relative to the given Program instance.
	* @see _exps
	*
	* @return 	Array of existing Expression instances
	*/
	public List<Expression> getExpressions(){
		return _exps;
	}
	
	/**
	* Adds an expression to the program by adding it to the collection _exps.
	* @see _exps
	*
	* @param	idx			Index of the Expression instance to be replaced
	* @param	expS		String that represents the Expression to add
	*/
	public void expressionAdd(int idx, String expS) throws ParserException, IndexOutOfBoundsException{
		NewParser parser = new NewParser();
		Expression exp = parser.parseString(expS, this);
		_exps.add(idx, exp);
	}

	/**
	* Replaces an expression of the program by replacing the collection object
	* with the new Expression reference.
	* @see _exps
	*
	* @param	idx			Index of the Expression instance to be replaced
	* @param	expS		String that represents the Expression to replace
	*/
	public void expressionReplace(int idx, String expS) throws ParserException, IndexOutOfBoundsException{
		NewParser parser = new NewParser();
		Expression exp = parser.parseString(expS, this);
		_exps.set(idx, exp);
	}

	/**
	* Sets the _exps field to be the collection given as argument.
	* @see _exps
	*
	* @param	exps		Collection of expressions to be set as the program's
	*						_exps field
	*/
	public void set(Collection<Expression> exps){
		for (Expression aux : exps)
			_exps.add(aux);
	}

	/**
	* Returns the name of the program.
	* @see _name
	*
	* @return 	String that represents the name of the program
	*/
	public String getName(){
		return _name;
	}

	/**
	* Returns the Intepreter instance reference that has this Program instance.
	* @see _interp
	*
	* @return 	Interpreter instance
	*/
	public Interpreter getInterpreter(){
		return _interp;
	}

	/**
	* Returs the program as a String. Concatenates every Expression instance's
	* text (from Expression class method getAsText()) with line separators
	* to be easily read. Used when writting the program to a file so it can be
	* read and interpreted by another interpreter.
	* @see _exps
	*
	* @return	String that represents the program as text
	*/
	public String getAsText(){
		String res = "";
		for (Expression exp : _exps)
			res += exp.getAsText() + "\n";
		return res;
	}

	/**
	* Adds and Identifier to the Identifier Map in the Interpreter that has
	* the program. Method only created for encapsulation purposes.
	* @see _exps
	*
	* param		id			Identifier to add to the Map
	*/
	public void identifierAdd(Identifier id){
		_interp.identifierAdd(id);
	}

	/**
	* Sets and Identifier value in the Identifier Map in the Interpreter that 
	* has the program. Method only created for encapsulation purposes.
	* @see _exps
	*
	* param		id			Identifier which to set
	* param 	value		Literal to associate with the Identifier
	*/
	public void setIdentifierValue(Identifier id, Literal value){
		_interp.setIdentifierValue(id, value);
	}

	/**
	* Returns the value of the Identifier. The value is stored in the Identifier
	* Map in the Interpreter that has the program. Method only created for 
	* encapsulation purposes.
	* @see _exps
	*
	* param		id			Identifier from which to get the value
	* return	The Literal associated with the given Identifier
	*/
	public Literal getIdentifierValue(Identifier id){
		return _interp.getIdentifierValue(id);
	}

	/**
	* Returns a String that represents the names of all the Identifiers of the
	* Program instance.
	*
	* return	String with all the Identifier instance's names
	*/
	public String getAllIdentifiers(){
		FirstVisitorImp fvi = new FirstVisitorImp();
		String res = "";
		for (Expression exp : _exps)
			exp.accept(fvi);
		TreeSet<Identifier> set = fvi.getIdentifierSet();
		for (Identifier id : set)
			res += id.getAsText() + "\n";
		return res;
	}

	/**
	* Returns a String that represents the names of all the Identifiers of the
	* Program instance that weren's initializes with Set operator.
	*
	* return	String with all the names of the Identifier instances that
	*			weren't initializes with Set
	*/
	public String getUninitializedIdentifiers(){
		FirstVisitorImp fvi = new FirstVisitorImp();
		SecondVisitorImp svi = new SecondVisitorImp();
		String res = "";
		for (Expression exp : _exps){
			exp.accept(fvi);
			exp.accept(svi);
		}
		TreeSet<Identifier> set1 = fvi.getIdentifierSet();
		TreeSet<Identifier> set2 = svi.getIdentifierSet();
		set1.removeAll(set2);
		for (Identifier id : set1)
			res += id.getAsText() + "\n";
		return res;
	}
}