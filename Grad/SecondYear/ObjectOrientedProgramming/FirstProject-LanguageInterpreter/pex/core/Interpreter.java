package pex.core;

import java.util.*;
import java.io.BufferedWriter;
import java.io.FileWriter;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;

import java.io.IOException;
import java.io.FileNotFoundException;
import java.lang.ClassNotFoundException;
import pex.parser.BadSourceException;
import pex.parser.BadNumberException;
import pex.parser.InvalidExpressionException;

import pex.AppIO;
import pex.parser.NewParser;
import pex.core.Program;
import pex.core.expression.Literal;
import pex.core.expression.literal.IntegerLiteral;
import pex.core.expression.Identifier;
import pex.core.IdentifierComparator;

public class Interpreter implements java.io.Serializable{
	private String _fileName = "";
	private AppIO _appIO;
	private Map<String, Program> _mapProgram;
	private Map<Identifier, Literal> _mapIdentifierAll;
	
	public Interpreter(AppIO appIO){
		_appIO = appIO;
		_mapProgram = new HashMap<String, Program>();
		_mapIdentifierAll = new TreeMap<Identifier, Literal>(new IdentifierComparator());
	}

	public String getFileName(){
		return _fileName;
	}

	public void setFileName(String filename){
		_fileName = filename;
	}
	
	public void programAdd(Program prog){ 
		_mapProgram.put(prog.getName(), prog);
	}
	
	public Program programFind(String name){
		if (_mapProgram.containsKey(name))
			return _mapProgram.get(name);
		return null;
	}

	public void identifierAdd(Identifier id){
		_mapIdentifierAll.put(id, null);
	}
	
	public void setIdentifierValue(Identifier id, Literal value){
		_mapIdentifierAll.put(id, value);
	}

	public Literal getIdentifierValue(Identifier id){
		return _mapIdentifierAll.get(id);
	}

	public Literal programExecute(String name){
		return programFind(name).execute();
	}

	public AppIO getAppIO(){
		return _appIO;
	}
	
	public void interpreterSave() throws IOException{
		ObjectOutputStream oos = new ObjectOutputStream(new FileOutputStream(_fileName));
		oos.writeObject(this);
		oos.close();
	}
	
	public static Interpreter interpreterOpen(String filepath) throws FileNotFoundException, IOException, ClassNotFoundException{
		ObjectInputStream ois = new ObjectInputStream(new FileInputStream(filepath));
		Interpreter interp = (Interpreter)ois.readObject();
		ois.close();
		interp.setFileName(filepath);
		return interp;
	}
}