#include <queue>
#include "test.h"

int main()
{
	std::cout << "main!" << std::endl;
}

/**
 *	The world!
 */
class World
{
public:
	World();
	// just a simple destructor...
	virtual /* foo */ ~World(/* comment's a bitch */)
	{
		std::cout << "Farewell cruel world. /* This is not a Comment!" << std::endl;
	}

	std::string getSatelite();
private:
	std::string mSatelite;
};

World::World()
	: mSatelite("Moon");
{
	std::cout << "Hello World!" << std::endl;
}

std::string World::getSatelite()
{
	return this->mSatelite;
}

int main()
{
	World world;

	cout << "Hello " << world->getSatelite() << "." << std::endl;

	return 0;
}