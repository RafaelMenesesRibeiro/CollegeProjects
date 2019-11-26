package pex.core.saver;

import java.io.IOException;

import pex.core.Interpreter;
import pex.core.Program;

public interface ProgramWriter{
	public Program programWrite(Interpreter interp, String progName, String filepath) throws IOException;
}