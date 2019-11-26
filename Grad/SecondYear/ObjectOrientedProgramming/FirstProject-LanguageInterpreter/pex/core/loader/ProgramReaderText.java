package pex.core.loader;

import java.io.IOException;
import pex.parser.BadSourceException;
import pex.parser.BadNumberException;
import pex.parser.InvalidExpressionException;

import pex.parser.NewParser;
import pex.core.Interpreter;
import pex.core.Program;

public class ProgramReaderText implements ProgramReader{
	@Override
	public void programRead(Interpreter interp, String filepath) throws BadSourceException, BadNumberException, InvalidExpressionException{
		NewParser parser = new NewParser();
		Program prog = parser.parseFile(interp, filepath, filepath);
		interp.programAdd(prog);
	}
}