package pex.core;

import java.util.*;

import pex.core.expression.Identifier;

public class IdentifierComparator implements java.io.Serializable, Comparator<Identifier>{
	public int compare(Identifier l1, Identifier l2){
		return l1.getName().compareTo(l2.getName());
	}
}