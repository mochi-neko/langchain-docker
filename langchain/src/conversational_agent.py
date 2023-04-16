from langchain import GoogleSearchAPIWrapper
from langchain.agents import load_tools, ZeroShotAgent, initialize_agent, AgentType, Tool
import langchain.llms
from langchain.schema import BaseMemory


def setup_agent(llm : langchain.llms.BaseLLM, memory: BaseMemory):
    search = GoogleSearchAPIWrapper()
    tools = [
        Tool(
            name="Current Search",
            func=search.run,
            description="useful for when you need to answer questions about current events or the current state of the world"
        ),
    ]

    agent_executor = initialize_agent(
        llm=llm,
        tools=tools,
        agent=AgentType.CONVERSATIONAL_REACT_DESCRIPTION,
        memory=memory,
        verbose=True)

    return agent_executor