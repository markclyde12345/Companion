<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="project.ForgotPasswordPage"
             Title="Forgot Password">

    <VerticalStackLayout Padding="30" Spacing="20">
        <Label Text="Reset Your Password" FontSize="24" HorizontalOptions="Center"/>
        <Entry x:Name="EmailEntry" Placeholder="Enter your email"/>
        <Button Text="Send Reset Link" Clicked="OnSendResetClicked"/>
        <Label x:Name="MessageLabel" TextColor="Red" HorizontalOptions="Center"/>
    </VerticalStackLayout>
</ContentPage>