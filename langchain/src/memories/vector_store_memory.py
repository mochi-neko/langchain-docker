from langchain.embeddings import OpenAIEmbeddings
from langchain.memory import VectorStoreRetrieverMemory
from langchain.vectorstores import Chroma


def setup_memory() -> VectorStoreRetrieverMemory:
    # Setup vector DB
    embeddings = OpenAIEmbeddings()
    vector_db = Chroma(
        collection_name="long_term_memory",
        embedding_function=embeddings,
        persist_directory="chromadb/long_term_memory")
    vector_db.persist()

    # Convert to retriever
    retriever = vector_db.as_retriever(search_kwargs=dict(k=5))

    # Convert to memory
    return VectorStoreRetrieverMemory(retriever=retriever)
