package pex.core.loader;

import java.io.IOException;
import pex.parser.BadSourceException;
import pex.parser.BadNumberException;
import pex.parser.InvalidExpressionException;

import pex.core.Interpreter;
import pex.core.Program;

public interface ProgramReader{
	public void programRead(Interpreter interp, String filepath)  throws BadSourceException, BadNumberException, InvalidExpressionException;
}