namespace Lumen.Studio {
	public interface IEditable : IUndoable, IRedoable, ICopyable, IPasteable, ICutable, IDeleteable {

	}

	public interface IUndoable {
		void Undo();
	}

	public interface IRedoable {
		void Redo();
	}

	public interface ICopyable {
		void Copy();
	}

	public interface IPasteable {
		void Paste();
	}

	public interface ICutable {
		void Cut();
	}

	public interface IDeleteable {
		void Delete();
	}
}