/**/
/***************************************************/
/*                                                 */
/* Demo of Cast operator                           */
/*                                                 */
/***************************************************/

//'c'
//'\''
//"g\\\""
//"text"
//"test\"string"
//0
//12345
//01234
//0.54
//159.951
//0xAF
//0xA1
//0xa1

#include <stdio.h>

std::map<std::string,int> values;

int main ()               /* Use int float and char */

{
	float x;
	int i;
	char ch;

	x = 2.345;
	i = (int) x;
	ch = (char) x;
	printf ("From float x =%f i =%d ch =%c\n",x,i,ch);

	i = 45;
	x = (float) i;
	ch = (char) i;
	printf ("From int i=%d x=%f ch=%c\n",i,x,ch);

	ch = '*';
	i = (int) ch;
	x = (float) ch;
	printf ("From char ch=%c i=%d x=%f\n",ch,i,x);
	return 0;
}

