import langchain.llms
from langchain import GoogleSearchAPIWrapper
from langchain.agents import initialize_agent, AgentType, Tool
from langchain.schema import BaseMemory


def setup_agent(llm : langchain.llms.BaseLLM, memory: BaseMemory):
    search = GoogleSearchAPIWrapper()

    tools = [
        Tool(
            name="Google Search",
            func=search.run,
            description="useful for when you need to answer questions about current events, the current state of the world or what you don't know."
        ),
    ]

    agent = initialize_agent(
        llm=llm,
        tools=tools,
        agent=AgentType.CONVERSATIONAL_REACT_DESCRIPTION,
        memory=memory,
        verbose=True)

    return agent