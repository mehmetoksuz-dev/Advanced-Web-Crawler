   M S B A M L   `   `   `  ����F  BWeb Crawler, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:Web_CrawlerWeb_Crawler  5   	       Web_Crawler.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:Web_Crawler     5   	     � Title$  Web Crawler��5   	   $	��886��6   $	��564��6)    � 
ResizeMode$ 	CanResize=�66    � WindowStartupLocation$ CenterScreen=�6M    � WindowState$ Normal=�6r   ���	#FF0B1A24.��5	      � ���	#FF0B1D24.+�5
   
   �� -   5         ��Name$ 
tabControl��5
      ���	#FF0B1D24
��� ���.@�5      �� $��Crawler��6   
��� ��� J� OpacityMask �	#FFA94141$��-2,-2,-23,0q�6k   $��Century Gothic)�6�   $��13(�6�   ��6T   Z� ��5      �� 5      .��5      � ���	#FF0B1D24.+�5      �� -   5         ��Name$ lstUrls��5      $��10,292,10,18q�6/   $��Century Gothic)�6E   $��13(�6a   �� 5      -   5         ��Name$ btnStartCrawling��5      $��Start To Crawl Website��67   $
��Left=�6Y   $��
10,228,0,0q�6t   $	��Top=�6�   $��54��6�   $	��180��6�   $��Center=�6�   ���	#FF0B1D24
�������
��� ���$��2,2,2,2q�68  $��Century Gothic)�6R  $
��Bold"�6n  $��13(�6�  �� 5      -   5        $ btnStopCrawling��5      $��Stop Crawling��66   $
��Left=�6N   $	��Top=�6i   $��54��6�   $	��139��6�   $��204,228,0,0q�6�   $��2,2,2,2q�6�   
��� ���
����������	#FF0B1D24$��Century Gothic)�6%  $��13(�6A  $
��Bold"�6O  �� 5      -   5        $ lstLinks��5      $��262,33,272,366q�60   �� 5      -   5        $ 
btnAddLink��5      $��Add Link��61   $
��Left=�6D   $��	10,67,0,0q�6_   $	��Top=�6r   $��53��6�   $��82��6�   
���q�<�
�������$��0,0,0,0q�6�   ���	#FF52C5D6$��Century Gothic)�6  $��13(�67  $
��Bold"�6E  �� 5      -   5         ��Name$ txtLinks��5      $
��Left=�61   $��	10,33,0,0q�6L   	 �� TextWrapping$
	 Wrap=�6_   $	��Top=�6s   $	��235��6�   
 � ToolTip$
  ��6�   $��24��6�   �� 5      -   5        $ btnDeletLink��5      $��Delete Link��63   $
��Left=�6I   $��
113,67,0,0q�6d   $	��Top=�6x   $��53��6�   $��82��6�   ���	#FFB44417
�������$��Century Gothic)�6�   $��13(�6  $
��Bold"�6  �� 5      -	   5        $ txtNumberOfTasks��5      $
��Left=�69   $��
686,33,0,0q�6T   $
	 Wrap=�6h   $	��Top=�6|   $	��163��6�   $
  ��6�   $��24��6�   �� 5      -
   5        $ 
btnSetTask��5      $	��Set��61   $
��Left=�6?   $	��Top=�6Z   $��37��6r   $��70��6~   $��
735,67,0,0q�6�   $��2,2,2,2q�6�   
��� ���
����������	#FF0B1D24$��Century Gothic)�6�   $��13(�6  $
��Bold"�6   62  �� 5      -   5         ��Name$ label��5      $��Century Gothic)�6+   $��Input of Links��6G   $
��Left=�6`   $��10,0,0,0q�6{   $	��Top=�6�   
��� ���$
��Bold"�6�   $��15(�6�   �� 5      -   5        $ 
label_Copy��5      $��Century Gothic)�60   $��Set Number Of Tasks��6L   $
��Left=�6j   $��	691,0,0,0q�6�   $	��Top=�6�   
��� ���$
��Bold"�6�   $��15(�6�   �� 5      -   5        $ label_Copy1��5      $��Century Gothic)�61   $��List of Links��6M   $
��Left=�6e   $��	262,0,0,0q�6�   $	��Top=�6�   
��� ���$
��Bold"�6�   $��15(�6�   5      5      �� 5      $��
Statistics��6   $��22,-2,-26,0q�6*   
������ 
��� ���$��Century Gothic)�6l    �	#FFA94141 J� RenderTransformOrigin$ 0.744,0.566&�6�   $��13(�6�   ��6�   Z� .��5       � ���	#FF0B1D22.+�5!      �� -   5        $ lstStatistics��5!      $��Center=�65   $��10,10,10,10q�6Y   5"      5#      5$      5%      5&      