
#include<stdio.h>
#include<math.h>

#define N 10             //NΪҪ��ϵ����ݵĸ���
#define M 6              //MΪʵ�������
#define pi 2 * acos(0.0)
#define sta 400
double X[N];
double Y[N];

double K;               //���ֱ�ߵ�б��
double R;               //���ֱ�ߵĽؾ�
double x_sum_average;   //���� X[N] ��Ԫ����� ����ƽ��ֵ
double y_sum_average;   //���� Y[N] ��Ԫ����� ����ƽ��ֵ
double x_square_sum;    //���� X[N] ����Ԫ�ص�ƽ��ֵ
double x_multiply_y;    //���� X[N]��Y[N]��ӦԪ�صĳ�
double Sum_Average(double *d,int len)
{
	int i = 0;
	double z = 0.0;
	for(i = 0;i < len;i ++)
	{
		z += d[i];
	}
	z = z / N;
	return z;
}
double X_Y_By(double *m,double *n,int len)
{
	int i = 0;
	double z = 0.0;
	for(i = 0;i < len;i ++)
	{
		z += m[i] * n[i];
	}
	return z;
}

double Squre_sum(double *c,int len)
{
	int i = 0;
	double z = 0.0;
	for(i = 0;i < len;i ++)
	{
		z += c[i] * c[i];
	}
	return z;
}

double Line_Fit(int len)
{
	x_sum_average = Sum_Average(X,len);
	y_sum_average = Sum_Average(Y,len);
	x_square_sum = Squre_sum(X,len);
	x_multiply_y = X_Y_By(X,Y,len);
	K = ( x_multiply_y - len * x_sum_average * y_sum_average)/( x_square_sum - len * x_sum_average*x_sum_average );
	R = y_sum_average - K * x_sum_average;
	//printf("K = %f\n",K);
	//printf("R = %f\n",R);
	if(atan(K) <= 0) return 0;
	if(K > sta) return ((1 - fabs(atan(K) - atan(sta)) / (pi / 2 - atan(sta))) * 0.3 + 0.7) * 100;
	else return (1 - fabs(atan(K) - atan(sta)) / atan(sta)) * 100;
}

int main(){

    freopen("E:\\test\\in.txt","r",stdin);
	int i,j,k;
	double res[6],datay[M * N];
	for(i = 0;i < M * N;++ i)
        scanf("%lf",&datay[i]);
	for(j = 0;j < M;++ j){
	    for(k = 0,i = j * N;i < (j + 1) * N;++ i){
            X[k] = k + 1;
            Y[k ++] = datay[i];
	    }
        res[j] = Line_Fit(N);
        //printf("���ƶ�ƥ����Ϊ: ");
        //printf("%lf%%\n",res[j]);
	}
	return 0;
}
